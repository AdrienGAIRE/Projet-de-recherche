using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ObjectCreation : MonoBehaviour {

    public GameObject vehicule;
    public float zoom;
    public int x_shift;
    public int y_shift;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void UpdateVehicule (string info)
    {
        // First, we split the String to seperate the informations.
        string name = info.Split(':')[0];
        Debug.Log(name);
        string coordinates = info.Split(':')[1];
        Debug.Log(coordinates);

        // Then, the position is calculated, accounting for the size of the map.
        Vector3 position = new Vector3((float) (float.Parse(coordinates.Split(';')[0], CultureInfo.InvariantCulture.NumberFormat)*zoom+x_shift), 0, (float) (float.Parse(coordinates.Split(';')[1], CultureInfo.InvariantCulture.NumberFormat)*zoom+ y_shift));

        // We look if the object already exists, creating it if not.
        GameObject Temp = GameObject.Find(name);
        if (Temp == null)
        {
            Temp = Instantiate(vehicule, position, new Quaternion());
            Temp.name = name;
        }
        // Finally, the new coordinates are applied.
        Temp.transform.position = position;

    }
}
