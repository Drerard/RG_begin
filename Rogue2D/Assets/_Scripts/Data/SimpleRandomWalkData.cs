using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SimpleRndWParameters", menuName = "PCG/SimplRandomWalkData", order = 51)]
public class SimpleRandomWalkData : ScriptableObject
{
    public int iterations = 10;
    public int walkLenght = 10;
    public bool startRandomlyEachIteration = true;
}
