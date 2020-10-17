using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    [HideInInspector]
    public int row, column;
    [HideInInspector]
    public bool amIMainCube;
    [SerializeField]
    private ParticleSystem particle;
    private LevelManagerScript levelManager;
    // Start is called before the first frame update
    void Awake()
    {
        levelManager = FindObjectOfType<LevelManagerScript>();
    }

    public void CheckConnectedCubes()
    {
        if(row < 9 && levelManager.currentCubesIndices[row + 1, column] == 3) //sağındaki kübe bakıyor
        {
            levelManager.currentCubesIndices[row + 1, column] = 2;
            levelManager.currentCubes[row + 1, column].GetComponent<CubeScript>().CheckConnectedCubes();
        }
        if (row > 0 && levelManager.currentCubesIndices[row - 1, column] == 3) //solundaki kübe bakıyor
        {
            levelManager.currentCubesIndices[row - 1, column] = 2;
            levelManager.currentCubes[row - 1, column].GetComponent<CubeScript>().CheckConnectedCubes();
        }
        if (column < 9 && levelManager.currentCubesIndices[row, column + 1] == 3) //üstündeki kübe bakıyor
        {
            levelManager.currentCubesIndices[row, column + 1] = 2;
            levelManager.currentCubes[row, column + 1].GetComponent<CubeScript>().CheckConnectedCubes();
        }
        if (column > 0 && levelManager.currentCubesIndices[row, column - 1] == 3)  //altındaki kübe bakıyor
        {
            levelManager.currentCubesIndices[row, column - 1] = 2;
            levelManager.currentCubes[row, column - 1].GetComponent<CubeScript>().CheckConnectedCubes();
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);   
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Obstacle"))
        {
            var par = Instantiate(particle, transform.position, Quaternion.Euler(-90, 0, 0));
            par.Emit(5);
            

            if (amIMainCube)
            {
                levelManager.Lose();
                Destroy();
            }
            else
            {
                levelManager.currentCubesIndices[row, column] = 0;

                levelManager.Checking();
             
               Destroy();
            }
        }
    }
}
