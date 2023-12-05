using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI; // Button sınıfını kullanabilmek için gerekli namespace

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using TMPro;


public class transform : MonoBehaviour
{
    public Button transform_button; // Buton referansı

    private List<Vector3> p_pointPositions = new List<Vector3>();
    private List<Vector3> q_pointPositions = new List<Vector3>();

    private List<GameObject> objects = new List<GameObject>();

    public GameObject prefab;

    public TextMeshProUGUI text1;

    public TextMeshProUGUI text2;
    void Start()
    {

        text1 = GameObject.Find("R").GetComponent<TextMeshProUGUI>();
        text2 = GameObject.Find("T").GetComponent<TextMeshProUGUI>();
        // Eğer buton referansını henüz atamadıysanız, kodun çalışması için buton referansını atayın
        if (transform_button == null)
        {
            // Örnek olarak, eğer buton sahnenizde ise bu şekilde atayabilirsiniz
            transform_button = GetComponent<Button>();
        }

        point pointclass = new point();
        p_pointPositions = pointclass.getList_p_points();
        q_pointPositions = pointclass.getList_q_points();

        // Butona tıklama event'i eklemek için AddListener kullanıyoruz
        transform_button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        foreach (var item in objects)
        {
            Destroy(item);
        }
        Debug.Log("Hello dünya!");
        //PrintPointPositions(p_pointPositions);

        int numIterations = 1000; // İterasyon sayısı
        List<int> bestIndices = new List<int>();
        float threshold = 15;
        int numOfCon = 0;

        for (int i = 0; i < numIterations; i++)
        {
            List<int> selectedIndices = GetRandomIndices(3, p_pointPositions.Count);
            
            Tuple<Matrix<double>, Vector<double>> result = findRandT(selectedIndices);
            
            Matrix<double> R = result.Item1;
            Vector<double> t = result.Item2;

            List<Vector3> p_primes = transformPoints(p_pointPositions.ToArray(), R, t);
            int tempNumOfCon = calculateCon(p_primes, q_pointPositions,threshold);

            if (tempNumOfCon > numOfCon)
            {
                numOfCon = tempNumOfCon;
                bestIndices = selectedIndices;
            }
        }

        Matrix<double> R1 = findRandT(bestIndices).Item1;
        Vector<double> t1 = findRandT(bestIndices).Item2;

        Debug.Log("Rotation Matrix:\n" + R1.ToString());
        Debug.Log("Translation Vector:\n" + t1.ToString());


        text1.text = "Rotation Matrix:\n" + R1.ToString();
        text2.text = "Translation Vector:\n" + t1.ToString();

        


        List <Vector3> last = transformPoints(p_pointPositions.ToArray(), R1, t1);
        for (int i = 0; i < last.Count(); i++)
        {
            DrawLine(p_pointPositions[i], last[i]);
            
            objects.Add(Instantiate(prefab, last[i], Quaternion.identity));
        }

        // Use the bestIndices and perform further operations

    }

    void DrawLine(Vector3 startPoint, Vector3 endPoint)
    {
        // Çizgi oluşturma işlemi
        LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // Çizgi iki noktadan oluşacak

        // Çizgi rengi ve kalınlığı ayarları
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // Başlangıç ve bitiş noktalarını belirleme
        lineRenderer.SetPosition(0, startPoint); // Çizginin başlangıç noktası
        lineRenderer.SetPosition(1, endPoint); // Çizginin bitiş noktası
    }

    List<Vector3> transformPoints(Vector3[] positions, Matrix<double> rotationMatrix, Vector<double> translationVector)
    {
        List<Vector3> transformedPoints = new List<Vector3>();

        foreach (Vector3 point in positions)
        {
            Vector<double> p = CreateVector.DenseOfArray(new double[] { point.x, point.y, point.z });
            Vector<double> transformedPoint = (rotationMatrix * p) + translationVector;

            transformedPoints.Add(new Vector3((float)transformedPoint[0], (float)transformedPoint[1], (float)transformedPoint[2]));
        }

        return transformedPoints;
    }


    List<int> GetRandomIndices(int count, int maxIndex)
    {
        List<int> indices = new List<int>();
        while (indices.Count < count)
        {
            
            int randomIndex = UnityEngine.Random.Range(0, maxIndex);
            if (!indices.Contains(randomIndex))
            {
                indices.Add(randomIndex);
            }
        }
        return indices;
    }
    
    Tuple<Matrix<double>, Vector<double>> findRandT(List<int> selectedIndices){

        int i = 0;
        Matrix<double> points1 = Matrix<double>.Build.DenseOfRowArrays(
            new double[] { p_pointPositions[selectedIndices[0]].x, p_pointPositions[selectedIndices[0]].y, p_pointPositions[selectedIndices[0]].z },
            new double[] { p_pointPositions[selectedIndices[1]].x, p_pointPositions[selectedIndices[1]].y, p_pointPositions[selectedIndices[1]].z },
            new double[] { p_pointPositions[selectedIndices[2]].x, p_pointPositions[selectedIndices[2]].y, p_pointPositions[selectedIndices[2]].z }
        );

        Matrix<double> points2 = Matrix<double>.Build.DenseOfRowArrays(
            new double[] { q_pointPositions[selectedIndices[0]].x, q_pointPositions[selectedIndices[0]].y, q_pointPositions[selectedIndices[0]].z },
            new double[] { q_pointPositions[selectedIndices[1]].x, q_pointPositions[selectedIndices[1]].y, q_pointPositions[selectedIndices[1]].z },
            new double[] { q_pointPositions[selectedIndices[2]].x, q_pointPositions[selectedIndices[2]].y, q_pointPositions[selectedIndices[2]].z }
        );

       
        Vector<double> centroid1 = points1.ColumnSums() / points1.RowCount;
        Vector<double> centroid2 = points2.ColumnSums() / points2.RowCount;

       
        Matrix<double> centeredPoints1 = points1.Subtract(Matrix<double>.Build.DenseOfRowVectors(new[] { centroid1, centroid1, centroid1 }));
        Matrix<double> centeredPoints2 = points2.Subtract(Matrix<double>.Build.DenseOfRowVectors(new[] { centroid2, centroid2, centroid2 }));

        
        Matrix<double> H = centeredPoints2.TransposeAndMultiply(centeredPoints1);

        
        var svd = H.Svd();
        Matrix<double> R = svd.U * svd.VT;

       
        Vector<double> t = centroid2 - (R * centroid1);

        
        Debug.Log("Rotation Matrix:\n" + R.ToString());
        Debug.Log("Translation Vector:\n" + t.ToString());

        return Tuple.Create(R, t);

        
        // Matrix<double> transformedPoints = (points1 * R.Transpose()) + Matrix<double>.Build.DenseOfRowVectors(new[] { t, t, t });

        // List <Vector3> result = new List<Vector3>();


        // for (int i = 0; i < transformedPoints.RowCount; i++)
        // {
        //     Vector3 position = new Vector3((float)transformedPoints[i, 0], (float)transformedPoints[i, 1], (float)transformedPoints[i, 2]);
        //     result.Add(position);
            
        //     Instantiate(prefab, position, Quaternion.identity);
        // }

        
        // Debug.Log("Transformed Points:\n" + transformedPoints.ToString());

        // return result;
    }


    int calculateCon(List<Vector3> p_primes, List<Vector3> q,double treshold)
    {
        int n = p_primes.Count();
        if(q.Count()<n){n = q.Count();}
        
        int count = 0;
        
        for (int i=0;i<n;i++){
            
            double mse = 0.0;
            
            mse += Vector3.SqrMagnitude(p_primes[i] - q[i]);
    
            mse /= 3; 

            if(mse < treshold){ count++;}

            
        }
        return count;
    }

     // Update is called once per frame
    void Update()
    {
        
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

  
}
