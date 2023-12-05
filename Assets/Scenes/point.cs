using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;
using System;
using System.IO;

public class point : MonoBehaviour
{
    public GameObject pPrefab; // p.txt'deki noktaları temsil etmek için kullanılacak prefab
    public GameObject qPrefab; // q.txt'deki noktaları temsil etmek için kullanılacak prefab

    

    private static List<Vector3> p_pointPositions = new List<Vector3>();
    private static List<Vector3> q_pointPositions = new List<Vector3>();



    void Start()
    {
        ShowPoints("p.txt", pPrefab,p_pointPositions);
        ShowPoints("q.txt", qPrefab,q_pointPositions);
        //PrintPointPositions(p_pointPositions);
        Debug.Log(p_pointPositions.Count);
        Debug.Log(q_pointPositions.Count);
    }

    public List<Vector3> getList_q_points(){
        return q_pointPositions;
    }

    public List<Vector3> getList_p_points(){
        return p_pointPositions;
    }



    void PrintPointPositions(List<Vector3> points)
    {
        // pointPositions listesindeki koordinatları yazdırmak için döngü
        Debug.Log("Okunan Nokta Koordinatları:");
        foreach (Vector3 position in points)
        {
            Debug.Log("x: " + position.x + ", y: " + position.y + ", z: " + position.z);
        }
    }

    void ShowPoints(string fileName, GameObject prefab,List<Vector3> pointPositions)
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        if (File.Exists(filePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                int pointCount = int.Parse(lines[0]); // İlk satırda nokta sayısı var

                for (int i = 1; i <= pointCount; i++)
                {
                    string[] coordinates = lines[i].Split(' ');

                    if (coordinates.Length >= 3)
                    {
                        float x = float.Parse(coordinates[0]);
                        float y = float.Parse(coordinates[1]);
                        float z = float.Parse(coordinates[2]);

                        Vector3 pointPosition = new Vector3(x, y, z);
                        pointPositions.Add(pointPosition);

                        // Prefab kullanarak noktayı oluştur
                        GameObject point = Instantiate(prefab, pointPosition, Quaternion.identity);
                        
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Dosya okunurken bir hata oluştu: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Dosya bulunamadı: " + filePath);
        }
    }

    


    // Update is called once per frame
    void Update()
    {
        
    }
}
