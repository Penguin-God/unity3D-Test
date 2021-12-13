using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRandom : MonoBehaviour
{
    [SerializeField] GameObject prefebs = null;
    float circleRange = 10f; // 랜덤한 원의 범위

    [SerializeField] GameObject[] cube = null;
    [SerializeField] GameObject[] cube2 = null;
    private void Awake()
    {   
        //Quaternion randRotate = Random.rotation;
        //Quaternion randRotate2 = Random.rotationUniform;
        //transform.rotation = randRotate;

        //Vector3 randPos = (Vector3)Random.insideUnitCircle * circleRange;
        //Vector3 createPos = transform.position + randPos;
        //Instantiate(prefebs, createPos, Quaternion.identity);

        //float x = Random.Range(0, 1f);
        //float y = Random.Range(0, 1f);

        //Vector2 randSquarePos = new Vector2(x, y);

        

        //for (int i = 0; i < cube.Length; i++)
        //{
        //    Color randColor = Random.ColorHSV(1, 1, 0, 0.5f, 0f, 0);
        //    cube[i].transform.position += Vector3.up * 2 * i;
        //    cube[i].GetComponent<MeshRenderer>().material.color = randColor;
        //}

        //for (int i = 0; i < cube2.Length; i++)
        //{
        //    Color randColor2 = Random.ColorHSV(1, 1, 0.5f, 1, 1f, 1);
        //    cube2[i].transform.position += Vector3.forward * 2 * i;
        //    cube2[i].GetComponent<MeshRenderer>().material.color = randColor2;
        //}

        StartCoroutine(Co_CreateSphere());
    }

    IEnumerator Co_CreateSphere()
    {
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 1000; j++)
            {
                Vector3 ranpos = (Vector3)Random.insideUnitCircle * 10 + transform.position;
                GameObject obj = Instantiate(prefebs, ranpos, Quaternion.identity);
                Color randColor2 = Random.ColorHSV(0.1f, 0.1f, 0.5f, 1, 1f, 1f);
                obj.GetComponent<MeshRenderer>().material.color = randColor2;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}