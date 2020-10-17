using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Levels", order = 1)]
public class LevelScriptableObject : ScriptableObject
{
    public ArrayLayout cubes; // 0 = Boşluk, 1 = MainCube(kırmamamız gereken küp), 2 = Main küpe bağlı normal küp, 3 = Main küple bağlantısı olmayan küp demek.

    [Serializable]
    public struct ObstaclesProperties
    {
        public ObstacleType obstacleType;
        public Vector3 obstaclePosition;
        public Vector3 rotation;
    }
 
    public ObstaclesProperties[] thisLevelObstacles;
}
