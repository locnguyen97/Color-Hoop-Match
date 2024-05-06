using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public string color = "";

    public void SetData(Sprite cl , string slot)
    {
        color = slot.ToString();
        GetComponent<SpriteRenderer>().sprite = cl;
    }
    private void OnMouseDown()
    {
        if (GameManager.Instance.canDrag)
        {
            CheckAndMove();
        }
    }

    void CheckAndMove()
    {
        if (GameManager.Instance.curColor == "")
        {
            GameManager.Instance.curColor = color;
            GameManager.Instance.canDrag = false;
            var target = GameManager.Instance.GetCurLevel().gameObjectsSlot[GameManager.Instance.curSlot];
            Move(target.transform);
            GameManager.Instance.curSlot++;
        }
        else
        {
            if (GameManager.Instance.curColor == color)
            {
                GameManager.Instance.canDrag = false;
                var target = GameManager.Instance.GetCurLevel().gameObjectsSlot[GameManager.Instance.curSlot];
                Move(target.transform);
                GameManager.Instance.curSlot++;
            }
        }
    }
    
    public void Move(Transform target)
    {
        StartCoroutine(MoveToTarget(target));
    }

    IEnumerator MoveToTarget(Transform target)
    {
        var dis = Vector3.Distance(target.position , transform.position);
        var dir = target.position - transform.position;
        while (dis > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            transform.position = transform.position + dir * 0.013f;
            dis = Vector3.Distance(target.position , transform.position);
        }

        transform.position = target.position;
        transform.rotation = Quaternion.Euler(0,0,0);
        GameManager.Instance.listCurCar.Add(this);
        CheckOnMoveDone();
        //GameManager.Instance.GetCurLevel().RemoveObject(gameObject);
    }

    void CheckOnMoveDone()
    {
        if (GameManager.Instance.curSlot == 3)
        {
            foreach (var car in GameManager.Instance.listCurCar)
            {
                GameManager.Instance.GetCurLevel().RemoveObject(car.gameObject);
                Destroy(car.gameObject);
            }

            GameManager.Instance.curColor = "";
            GameManager.Instance.curSlot = 0;
            GameManager.Instance.listCurCar.Clear();
            var particleVFXs = GameManager.Instance.particleVFXs;
            GameObject explosion = Instantiate(particleVFXs[UnityEngine.Random.Range(0,particleVFXs.Count)], transform.position, transform.rotation);
            Destroy(explosion, .75f);
        }
        GameManager.Instance.EnableDrag();
    }
}
