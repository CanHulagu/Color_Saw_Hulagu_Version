using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagerScript : MonoBehaviour
{

    public LevelHolderScriptableObject levelHolder; 
    public int currentLevel; //kaçıncı leveldayız

    [SerializeField]
    private GameObject mainCubePrefab, normalCubePrefab, bigSawPrefab, littleSawPrefab, bigCylinderPrefab, LittleCylinderPrefab;
    [SerializeField]
    private ParticleSystem particle1, particle2; //konfeti efektleri
    [SerializeField]
    private GameObject tryAgain, nextLevel; //UI

    [HideInInspector]
    public int[,] currentCubesIndices; //Küplerin değerlerini tuttuğumuz 2d array( 0 = Boşluk, 1 = MainCube(kırmamamız gereken küp), 2 = Main küpe bağlı normal küp, 3 = Main küple bağlantısı olmayan küp demek)
    [HideInInspector]
    public GameObject[,] currentCubes; //Küpleri tuttuğumuz 2d array

    GameObject[] currentObstacles;  //levelda kullanılan engeller
    Vector3 preMousePos; 
    bool playing;

    // Start is called before the first frame update
    void Awake()
    {
        tryAgain.SetActive(false);
        nextLevel.SetActive(false);

        SetLevel(currentLevel);
        playing = true;
    }

    private void FixedUpdate()
    {
        if (playing) 
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                preMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 direction = preMousePos - Input.mousePosition;

                transform.position = new Vector3(transform.position.x + direction.y * Time.deltaTime, 0, transform.position.z - direction.x * Time.deltaTime);

                preMousePos = Input.mousePosition;
            }

            float x = Mathf.Clamp(transform.position.x, -15, 15);
            float z = Mathf.Clamp(transform.position.z, -10, 10);

            transform.position = new Vector3(x, 0, z);
        }    
    }

    void ClearLevel() //küpleri ve engelleri siler, pozisyonu sıfıra getirir
    {
        for (int row = 0; row < 10; row++)
        {
            for (int column = 0; column < 10; column++)
            {
                if (currentCubes[row, column] != null)
                    Destroy(currentCubes[row, column]);
            }
        }

        int obsNumber = currentObstacles.Length;

        for (int i = 0; i < obsNumber; i++)
        {
            Destroy(currentObstacles[i]);
        }

        tryAgain.SetActive(false);
        nextLevel.SetActive(false);

        transform.position = Vector3.zero;
    }

    void SetLevel(int index) //ScriptableObject verilerine göre levelı kurar
    {
        currentCubesIndices = new int[10, 10];
        currentCubes = new GameObject[10, 10];

        for (int row = 0; row < 10; row++)
        {
            for (int column = 0; column < 10; column++)
            {
                int i = currentCubesIndices[row, column] = levelHolder.levels[currentLevel].cubes.rows[row].row[column];  //scriptableObjectin 2D arrayini buradaki 2D arraye eşitliyor.

                if (i == 0)
                {
                    currentCubes[row, column] = null;
                }
                else if (i == 1)
                {
                    currentCubes[row, column] = Instantiate(mainCubePrefab, new Vector3(row, 0, column), Quaternion.identity,transform); //main cube spawn ediyor.
                    currentCubes[row, column].GetComponent<CubeScript>().row = row;
                    currentCubes[row, column].GetComponent<CubeScript>().column = column;
                    currentCubes[row, column].GetComponent<CubeScript>().amIMainCube = true;
                }
                else
                {
                    currentCubes[row, column] = Instantiate(normalCubePrefab, new Vector3(row, 0, column), Quaternion.identity,transform); //normal cube spawn ediyor.
                    currentCubes[row, column].GetComponent<CubeScript>().row = row;
                    currentCubes[row, column].GetComponent<CubeScript>().column = column;
                    currentCubes[row, column].GetComponent<CubeScript>().amIMainCube = false;
                }
            }
        }

        int obstacleNumber = levelHolder.levels[currentLevel].thisLevelObstacles.Length;
        currentObstacles = new GameObject[obstacleNumber];

        for (int i = 0; i < obstacleNumber; i++)   // Obstacleları belirlenen pozisyon ve rotasyonda spawnlar.
        {
            if (levelHolder.levels[currentLevel].thisLevelObstacles[i].obstacleType == ObstacleType.bigCylinder)
            {             
                var obs = Instantiate(bigCylinderPrefab, levelHolder.levels[currentLevel].thisLevelObstacles[i].obstaclePosition, Quaternion.Euler(levelHolder.levels[currentLevel].thisLevelObstacles[i].rotation));
                currentObstacles[i] = obs;
            }
            else if (levelHolder.levels[currentLevel].thisLevelObstacles[i].obstacleType == ObstacleType.bigSaw)
            {
                var obs = Instantiate(bigSawPrefab, levelHolder.levels[currentLevel].thisLevelObstacles[i].obstaclePosition, Quaternion.Euler(levelHolder.levels[currentLevel].thisLevelObstacles[i].rotation));
                currentObstacles[i] = obs;
            }
            else if (levelHolder.levels[currentLevel].thisLevelObstacles[i].obstacleType == ObstacleType.littleCylinder)
            {
                var obs = Instantiate(LittleCylinderPrefab, levelHolder.levels[currentLevel].thisLevelObstacles[i].obstaclePosition, Quaternion.Euler(levelHolder.levels[currentLevel].thisLevelObstacles[i].rotation));
                currentObstacles[i] = obs;
            }
            else if (levelHolder.levels[currentLevel].thisLevelObstacles[i].obstacleType == ObstacleType.littleSaw)
            {
                var obs = Instantiate(littleSawPrefab, levelHolder.levels[currentLevel].thisLevelObstacles[i].obstaclePosition, Quaternion.Euler(levelHolder.levels[currentLevel].thisLevelObstacles[i].rotation));
                currentObstacles[i] = obs;
            }
        }
    }

    public void Checking()
    {

        for (int row = 0; row < 10; row++) //İlk önce main küp olmayan bütün küpleri bağlı değil yapıyor
        {
            for (int column = 0; column < 10; column++)
            {
                if(currentCubesIndices[row, column] == 2)
                {
                    currentCubesIndices[row, column] = 3;
                }
            }
        }

        for (int row = 0; row < 10; row++)  //Sonra Main küplerden başlayan bir kontrol dalgasıyla bağlı olan küplerin indexi 3ten 2ye gelir
        {
            for (int column = 0; column < 10; column++)
            {
                int whatIsIndex = currentCubesIndices[row, column];

                if (whatIsIndex == 1) //Main küplerin CheckConnectedCubes methodunu başlatır.
                {
                    currentCubes[row, column].GetComponent<CubeScript>().CheckConnectedCubes();
                }
            }
        }

        DestroyUnconnectedCubes(); //Bir main cube ile bağlantısı olmayan küpleri yok eder


        bool win = true;

        for (int row = 0; row < 10; row++) //bütün normal küpler bittiyse kazanmış oluyoruz
        {
            for (int column = 0; column < 10; column++)
            {
                if (currentCubesIndices[row, column] == 2)
                {
                    win = false;
                }
            }
        }

        if (win)
        {
            playing = false;
            StartCoroutine("Win");
        }

    }

    void DestroyUnconnectedCubes()
    {
        for (int row = 0; row < 10; row++)
        {
            for (int column = 0; column < 10; column++)
            {
                if (currentCubesIndices[row, column] == 3)
                {                    
                    currentCubes[row, column].GetComponent<CubeScript>().Destroy();
                    currentCubesIndices[row, column] = 0;
                }
            }
        }
    }

    IEnumerator Win()
    {
        particle1.Play();
        particle2.Play();

        yield return new WaitForSeconds(1f);

        particle1.Stop();
        particle2.Stop();

        nextLevel.SetActive(true);
    }

    public void Lose()
    {
        tryAgain.SetActive(true);
        playing = false;
    }

    public void Restart()
    {
        playing = true;
        ClearLevel();
        SetLevel(currentLevel);
    }
    public void NextLevel()
    {
        playing = true;

        if (currentLevel < levelHolder.levels.Length-1)
            currentLevel++;
        else
            currentLevel = 0;

        ClearLevel();
        SetLevel(currentLevel);
    }
}
