using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackRange : MonoBehaviour
{
    public bool attack;
    public GameObject target;
    List<GameObject> targetList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            targetList.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            target = null;
    }

    void TargetFind()
    {
        float dis = 0;
        if(target != null)
            dis = Mathf.Sqrt(Mathf.Pow(transform.position.x - target.transform.position.x, 2) + Mathf.Pow(transform.position.y - target.transform.position.y, 2));

        for (int i=0;i<targetList.Count;i++)
        {
            float dist = Mathf.Sqrt(Mathf.Pow(transform.position.x - targetList[i].transform.position.x, 2) + Mathf.Pow(transform.position.y - targetList[i].transform.position.y, 2));

            if (target == null)
            {
                target = targetList[i];
                continue;
            }
            else if(dis>dist)
            {
                target = targetList[i];
                continue;
            }
        }
    }

}
