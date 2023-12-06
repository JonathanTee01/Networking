using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int id;
    public Vector2 position;
    public Vector2 velocity;
    public Queue<Vector2> positionData = new Queue<Vector2>();
    public Queue<Vector2> velocityData = new Queue<Vector2>();
    public bool updated = false;
    private float timeDiff = 0.0f;

    // Reference to rigid body
    Rigidbody2D self;

    // Update is called once per frame
    public void InitData(int playerId, Vector2 pos, Vector2 vel,  Rigidbody2D playerBody)
    {
        // Initialise player data
        id = playerId;
        position = pos;
        self = playerBody;
        velocity = vel;

        // Set the position and vleocity of the new player
        self.transform.position = pos;
        self.velocity = vel;

        // Initalise the queues for the other players
        // This will be used to predict and interpolate
        positionData.Enqueue(pos);
        velocityData.Enqueue(vel);
    }

    public void UpdateOtherData(int sec, int mil, Vector2 pos, Vector2 vel)
    {
        // Update the player data to say it received new data
        // Used to determine whether to predict or not
        // e.g. Whether a packet was lost
        updated = true; 
        position = pos;

        // Time between recieving packet and when it was sent
        // If the recieved values are larger than the current ones then we have gone over to the next minute/second since the packet was sent
        // Therefore add to them to account for this
        int currentSec = System.DateTime.Now.Second;
        if (currentSec < sec)
        {
            currentSec += 60;
        }
        
        int currentMil = System.DateTime.Now.Millisecond;
        if (currentMil < mil)
        {
            currentMil += 1000;
        } 
        timeDiff = (currentSec - sec) + ((currentMil - mil)/1000);

        //Add the newly received data to the queues to track previous iterations of the player
        positionData.Enqueue(pos);
        velocityData.Enqueue(vel);

        // If the data queue is at an addequate length then trim it
        // Only wouldn't apply to the first 3 movement packets or predictions
        if (positionData.Count > 3)
        {
            positionData.Dequeue();
            velocityData.Dequeue();
        }

        if (self.transform.position.y - pos.y < -1 || self.transform.position.y - pos.y > 2)
        {
            // Teleport because they have fallen off a platform or are airborne by a significant margin
            self.transform.position = pos;
            self.velocity = vel;
        }
        else if (self.transform.position.x - pos.x < -2 || self.transform.position.x - pos.x > 2)
        {
            // Harshly lerp because they are far off from where they should be
            // Adjust the position forcefully to get it back towards the actual
            self.transform.position = new Vector2(((pos.x + self.transform.position.x)/2),((pos.y + self.transform.position.y)/2));
            lerp(pos);
        }
        else
        {
            // lerp
            lerp(pos);
        }
    }

    public void UpdateOtherData()
    {
        // predict the new position and velocity
        if (positionData.Count > 3)
        {
            // Don't use timeDiff in the prediction as this function it isn't used to catch up but to find where we were moving towards
            PredictPos();
        }
        // If there isn't enough data to predict with then just use the current position
        // Not ideal but to pass isn't hard
        else
        {
            position = new Vector2(self.transform.position.x, self.transform.position.y);
        }

        // Add the actual current position to the queue
        // This is used rather than the predicted position in order to account for obstacles
        positionData.Enqueue(new Vector2(self.transform.position.x, self.transform.position.y));

        // Trim excess from the queue
        if (positionData.Count > 3)
        {
            positionData.Dequeue();
        }

        // No if statements this time because there is no new data to verify against
        // lerp
        lerp(position);
    }

    private void lerp(Vector2 pos)
    {
        float lerpSpeed = 50.0f;

        // Move current position towards actual
        // velocity += (actualPos - currentPos) * lerpSpeed ;
        velocity = new Vector2((pos.x - self.transform.position.x) * lerpSpeed, (pos.y - self.transform.position.y) * lerpSpeed/2);

        // Since we're lerping with velocity it should simulate gravity
        self.velocity = velocity;
    }

    private void PredictPos()
    {
        // Oldest position
        Vector2 pos3 = positionData.Peek();
        positionData.Enqueue(positionData.Dequeue());

        Vector2 pos2 = positionData.Peek();
        positionData.Enqueue(positionData.Dequeue());

        // Newest postion
        Vector2 pos1 = positionData.Peek();
        positionData.Enqueue(positionData.Dequeue());

        // Position is now the predicted from the change in the change of position between the last three recordered positions
        // e.g. based on the rate of change in displacement
        // Not directly applied to the queued position because this one doesn't take into account obstacles
        // This will be used for predicting the lerping though
        position += ((pos1 - pos2) - (pos2 - pos3));
    }

    // Update the players data if the local client
    public void UpdateData()
    {
        position = self.transform.position;
        velocity = self.velocity;
    }

    // Write data into the packet
    public void WriteData()
    {
        Client.packet.NewWrite();
        Client.packet.writer.Write(id);
        Client.packet.writer.Write(System.DateTime.Now.Second);
        Client.packet.writer.Write(System.DateTime.Now.Millisecond);
        Client.packet.writer.SetEnumId((int)ClientPackets.playerMovement);
        Client.packet.writer.Write(position);
        Client.packet.writer.Write(velocity);
    }
}
