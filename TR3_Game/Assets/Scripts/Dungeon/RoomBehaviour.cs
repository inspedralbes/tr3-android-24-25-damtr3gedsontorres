using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomBehaviour : MonoBehaviour
{

    public GameObject[] walls;
    public GameObject[] doors;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
public void UpdateRoom(bool[] status)
{
    if (doors == null || walls == null || doors.Length < 4 || walls.Length < 4)
    {
        Debug.LogError("Doors or Walls are not assigned correctly in RoomBehaviour!");
        return;
    }

    for (int i = 0; i < status.Length; i++)
    {
        if (doors[i] != null)
            doors[i].SetActive(status[i]);

        if (walls[i] != null)
            walls[i].SetActive(!status[i]);
    }
}

}
