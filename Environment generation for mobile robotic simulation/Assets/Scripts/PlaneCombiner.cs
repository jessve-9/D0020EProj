using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCombiner
{
    public static float[,] CombineMaxValues(float[,] plane1, float[,] plane2){
        int width = plane1.GetLength (0);
        int length = plane1.GetLength (1);
        float[,] combine = new float[width, length];
        for(int z = 0; z < length; z++){
            for(int x = 0; x < width; x++){
                combine[x,z] = Mathf.Max(plane1[x,z], plane2[x,z]);
            }
        }
        return combine;
    }

    public static float[,] CombineMinValues(float[,] plane1, float[,] plane2){
        int width = plane1.GetLength (0);
        int length = plane1.GetLength (1);
        float[,] combine = new float[width, length];
        for(int z = 0; z < length; z++){
            for(int x = 0; x < width; x++){
                combine[x,z] = Mathf.Min(plane1[x,z], plane2[x,z]);
            }
        }
        return combine;
    }
    public static float[,] CreateTiltZ(float[,] plane1, float tilt){
        int width = plane1.GetLength (0);
        int length = plane1.GetLength (1);
        float[,] combine = new float[width, length];
        float tiltHeight = Mathf.Tan((tilt)*(Mathf.PI/180))*length;
        for(int z = 0; z < length; z++){
            for(int x = 0; x < width; x++){
                combine[x,z] = plane1[x,z]+z*tiltHeight;
            }
        }
        return combine;
    }

    public static float[,] CreateTiltX(float[,] plane1, float tilt){
        int width = plane1.GetLength (0);
        int length = plane1.GetLength (1);
        float[,] combine = new float[width, length];
        float tiltHeight = Mathf.Tan((tilt)*(Mathf.PI/180))*width;
        for(int z = 0; z < length; z++){
            for(int x = 0; x < width; x++){
                combine[x,z] = plane1[x,z]+x*tiltHeight;
            }
        }
        return combine;
    }
}
