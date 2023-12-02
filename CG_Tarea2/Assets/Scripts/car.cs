using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour
{
    [Header("Car Movement")]
    [SerializeField] Vector3 speed;
    [SerializeField] Vector3 carScale;

    [Header("Wheel Settings")]
    [SerializeField] GameObject wheel;
    [SerializeField] List<Vector3> wheelPositions;
    [SerializeField] Vector3 wheelScale;

    Mesh mesh;
    Vector3[] verticesCar;
    Vector3[] newVerticesCar;

    List<GameObject> wheelObjects = new List<GameObject>();
    List<Mesh> meshWheel = new List<Mesh>();
    List<Vector3[]> verticesWheel = new List<Vector3[]>();
    List<Vector3[]> newVerticesWheel = new List<Vector3[]>();

    void Start(){
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        verticesCar = mesh.vertices;
        newVerticesCar = new Vector3[verticesCar.Length];
        // Create a new array to store the new vertices
        foreach (Vector3 wheelPosition in wheelPositions){
            // Copy the vertices to the new array
            GameObject wheel = Instantiate(this.wheel, Vector3.zero, Quaternion.identity);
            wheel.transform.position = transform.TransformPoint(wheelPosition); 
            wheelObjects.Add(wheel);
            // Allocate memory for the vertices
            Mesh wheelMesh = wheel.GetComponentInChildren<MeshFilter>().mesh;
            meshWheel.Add(wheelMesh);
            // Create a new array to store the new vertices
            verticesWheel.Add(wheelMesh.vertices);
            newVerticesWheel.Add(new Vector3[verticesWheel[0].Length]);
        }
    }

    void Update(){
        //Call the function to move the car
        Matrix4x4 MoveCar = this.MoveCar();
        ApplyCarTransform(MoveCar);
        //Call the function to move the wheels
        for (int i = 0; i < wheelObjects.Count; i++)
        {
            Matrix4x4 wheelTransformMatrix = MoveWheels(MoveCar, i);
            ApplyWheelTransform(wheelTransformMatrix, i);
        }
    }

    Matrix4x4 MoveCar()
    {   //Create a matrix to move the car
        Matrix4x4 moveMatrix = HW_Transforms.TranslationMat(speed.x * Time.time, speed.y * Time.time, speed.z * Time.time);
        
        if (speed.x != 0)
        {   //Create a matrix to rotate the car
            float angle = speed.x * Time.time * 10;
            Matrix4x4 rotateMatrix = HW_Transforms.RotateMat(angle, AXIS.Y);
            return moveMatrix * rotateMatrix;
        }
        else
        {
            return moveMatrix;
        }
    }

    Matrix4x4 MoveWheels(Matrix4x4 moveCar, int wheelPos)
    {   //Create the matrices to move and rotate the wheels
        Matrix4x4 scaleMatrix = HW_Transforms.ScaleMat(wheelScale.x, wheelScale.y, wheelScale.z);
        Matrix4x4 spawnRotateMatrix = HW_Transforms.RotateMat(90, AXIS.Y);
        Matrix4x4 rotateMatrix = HW_Transforms.RotateMat(90 * Time.time, AXIS.X);
        Matrix4x4 moveMatrix = HW_Transforms.TranslationMat(wheelPositions[wheelPos].x, wheelPositions[wheelPos].y, wheelPositions[wheelPos].z);
        //Return the matrices multiplied
        return moveCar * moveMatrix * rotateMatrix * spawnRotateMatrix * scaleMatrix;
    }


    void ApplyCarTransform(Matrix4x4 carTransformMatrix)
    {   //Apply the transformation to the car
        for (int i = 0; i < newVerticesCar.Length; i++)
        {
            //Create a vector to store the vertices
            Vector4 temp = new Vector4(verticesCar[i].x, verticesCar[i].y, verticesCar[i].z, 1);
            newVerticesCar[i] = carTransformMatrix * temp;
        }
        //Apply the transformation to the mesh of the car
        mesh.vertices = newVerticesCar;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void ApplyWheelTransform(Matrix4x4 moveWheels, int wheelPos)
    {   //Apply the transformation to the wheels
        for (int j = 0; j < newVerticesWheel[wheelPos].Length; j++)
        {
            //Create a vector to store the vertices
            Vector4 temp = new Vector4(verticesWheel[wheelPos][j].x, verticesWheel[wheelPos][j].y, verticesWheel[wheelPos][j].z, 1);
            newVerticesWheel[wheelPos][j] = moveWheels * temp;
        }
        //Apply the transformation to the mesh of the wheels
        meshWheel[wheelPos].vertices = newVerticesWheel[wheelPos];
        meshWheel[wheelPos].RecalculateNormals();
        meshWheel[wheelPos].RecalculateBounds();
    }
}






