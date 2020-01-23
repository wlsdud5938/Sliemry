using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreater : MonoBehaviour
{
    public class Tree
    {
        public Tree(int a, int b)
        {
            this.point[0] = a;
            this.point[1] = b;
        }
        public int[] point = new int[2];
        public int roomNum;
        public Tree r;
        public Tree l;
        public Tree t;
        public Tree b;
        public Tree parent;
        public int parentDoor = 0;
        public int childDoor = 0;
        public GameObject room;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
