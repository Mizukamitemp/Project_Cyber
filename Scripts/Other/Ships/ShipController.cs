using UnityEngine;

public class ShipController : MonoBehaviour
{
    public int maxPassengers = 12;
    public int actualPassengers = 0;
    public ShipStation passengerPlace1;
    public ShipStation passengerPlace2;
    public ShipStation passengerPlace3;
    public ShipStation passengerPlace4;
    public ShipStation passengerPlace5;
    public ShipStation passengerPlace6;
    public ShipStation passengerPlace7;
    public ShipStation passengerPlace8;
    public ShipStation passengerPlace9;
    public ShipStation passengerPlace10;
    public ShipStation passengerPlace11;
    public ShipStation passengerPlace12;
    private float steeringInput;
    private float steeringActual;
    private float speedInput;
    private float speedActual;
    private float timer=0;

    // Start is called before the first frame update
    public GameObject GetPassengerStation()
    {
        if (!passengerPlace1.IsOccupied)
        {
            actualPassengers++;
            passengerPlace1.IsOccupied = true;
            passengerPlace1.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace1.gameObject;
        }
        else if (!passengerPlace2.IsOccupied)
        {
            actualPassengers++;
            passengerPlace2.IsOccupied = true;
            passengerPlace2.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace2.gameObject;
        }
        else if (!passengerPlace3.IsOccupied)
        {
            actualPassengers++;
            passengerPlace3.IsOccupied = true;
            passengerPlace3.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace3.gameObject;
        }
        else if (!passengerPlace4.IsOccupied)
        {
            actualPassengers++;
            passengerPlace4.IsOccupied = true;
            passengerPlace4.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace4.gameObject;
        }
        else if (!passengerPlace5.IsOccupied)
        {
            actualPassengers++;
            passengerPlace5.IsOccupied = true;
            passengerPlace5.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace5.gameObject;
        }
        else if (!passengerPlace6.IsOccupied)
        {
            actualPassengers++;
            passengerPlace6.IsOccupied = true;
            passengerPlace6.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace6.gameObject;
        }
        else if (!passengerPlace7.IsOccupied)
        {
            actualPassengers++;
            passengerPlace7.IsOccupied = true;
            passengerPlace7.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace7.gameObject;
        }
        else if (!passengerPlace8.IsOccupied)
        {
            actualPassengers++;
            passengerPlace8.IsOccupied = true;
            passengerPlace8.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace8.gameObject;
        }
        else if (!passengerPlace9.IsOccupied)
        {
            actualPassengers++;
            passengerPlace9.IsOccupied = true;
            passengerPlace9.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace9.gameObject;
        }
        else if (!passengerPlace10.IsOccupied)
        {
            actualPassengers++;
            passengerPlace10.IsOccupied = true;
            passengerPlace10.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace10.gameObject;
        }
        else if (!passengerPlace11.IsOccupied)
        {
            actualPassengers++;
            passengerPlace11.IsOccupied = true;
            passengerPlace11.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace11.gameObject;
        }
        else if (!passengerPlace12.IsOccupied)
        {
            actualPassengers++;
            passengerPlace12.IsOccupied = true;
            passengerPlace12.GetComponent<CapsuleCollider>().enabled = false;
            return passengerPlace12.gameObject;
        }

        return null;
    }

    public void Unload()
    {
        actualPassengers--;
    }
    public void GetInputVector(Vector2 input) 
    {
        steeringInput = input.x;
        speedInput = input.y;
    }

    private void FixedUpdate()
    {
        timer = timer + Time.fixedDeltaTime;

        GetComponent<Rigidbody>().freezeRotation = true;

        steeringActual = Mathf.Clamp(steeringActual + steeringInput * Time.fixedDeltaTime * 0.3f, -1, 1);
        speedActual = Mathf.Clamp(speedActual + speedInput * Time.fixedDeltaTime * 0.3f, -1, 1);
        GetComponent<Rigidbody>().MoveRotation(transform.rotation * Quaternion.Euler(transform.up * steeringActual * 0.1f + transform.forward * Mathf.Sin(timer/1.6f-3.14f/2) /80 + transform.right * Mathf.Sin(timer/1.7f - 3.14f / 2) / 80 ) );
        GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * speedActual * 0.1f);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //Debug.Log("Touched a rail");
            speedActual = 0;//speedActual/3;
        }
    }


    public void GetMeInto(GameObject station, GameObject player)
    {
        player.GetComponent<PlayerStateManager>().SwitchCargoStation(station);
    }

    public bool CanAcceptPassenger()
    {
        if (actualPassengers < maxPassengers)
        { 
            return true; 
        }
        else 
        { 
            return false; 
        }
    
    }

}
