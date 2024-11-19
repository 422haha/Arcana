using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Hand : MonoBehaviour
{
    public GameObject unit;
    public List<GameObject> unitsList;                              // 내 손에 있는 모든 유닛 

    public Vector3 cardScale;
    [Range(0, 5f)] public float rotationDegree;
    [Range(0, 50f)] public float basicDistance;
    [Range(0, 50f)] public float additionalDistance;                // 특정 카드에 마우스 올리면 그 카드 좌우로 이동하는 거 
    [Range(0, 10f)] public float positionYDegree;
    public float moveDuration;

    public void AddUnit()
    {
        GameObject currentUnit = Instantiate(unit, this.transform);
        currentUnit.transform.localPosition = new Vector3(0f, 50f, 0f);
        unitsList.Add(currentUnit);
        SettingPositions();
    }

    public void UnitUsed()
    {
        for (int i = 0; i < unitsList.Count; i++) if (unitsList[i].transform.childCount == 0)
            {
                Destroy(unitsList[i]);
                unitsList.RemoveAt(i);
            }
        SettingPositions();
    }


    public void SettingPositions()
    {
        bool isOdd = unitsList.Count % 2 == 1 ? true : false;
        float halfCount = ((float)unitsList.Count / 2) - 0.5f;

        for (int i = 0; i < unitsList.Count; i++)
        {
            Vector3 thisCardPos = new Vector3(0f,50f,0);
            float thisCardRot = 0f;
            if (!isOdd)
            {
                thisCardPos.x = i < halfCount ? thisCardPos.x - (basicDistance / 2f) : thisCardPos.x + (basicDistance / 2f);
            }

            // print(unitsList.Count);
            int distanceCount = (int)(i - halfCount);
            thisCardPos.x = thisCardPos.x + distanceCount * basicDistance;
            thisCardPos.y = thisCardPos.y + -Mathf.Abs(distanceCount) * positionYDegree;
            if (i - halfCount > 0) thisCardRot = thisCardRot + -Mathf.CeilToInt(i - halfCount) * rotationDegree;
            else thisCardRot = thisCardRot + -Mathf.FloorToInt(i - halfCount) * rotationDegree;

            // 임시 보관 (그냥 일시적으로 이동하게 하는 거)
            // unitsList[i].transform.localPosition = thisCardPos;
            // unitsList[i].transform.rotation = Quaternion.Euler(0f, 0f, thisCardRot);

            //unitsList[i].GetComponent<A_HandUnit>().StartCoroutine(TranslateCard(unitsList[i].transform, thisCardPos, thisCardRot));
            StartCoroutine(TranslateCard(unitsList[i].transform, thisCardPos, thisCardRot, cardScale));

        }
    }

    public void SettingPositions(bool isPointerEnter)
    {
        bool isOdd = unitsList.Count % 2 == 1 ? true : false;
        float halfCount = ((float)unitsList.Count / 2) - 0.5f;

        // 몇 번이 골라진 건지 확인
        int idx = -1;
        for (int i = 0; i < unitsList.Count; i++) if (unitsList[i].transform.childCount == 0) idx = i;

        for (int i = 0; i < unitsList.Count; i++)
        {
            Vector3 thisCardPos = new Vector3(0f, 50f, 0);
            float thisCardRot = 0f;
            if (!isOdd)
            {
                thisCardPos.x = i < halfCount ? thisCardPos.x - (basicDistance / 2f) : thisCardPos.x + (basicDistance / 2f);
            }

            print(unitsList.Count);
            int distanceCount = (int)(i - halfCount);
            thisCardPos.x = thisCardPos.x + distanceCount * basicDistance;
            if (i < idx) // 카드 왼쪽 부분
            {
                // 바로 왼쪽일 경우 
                if (i == idx - 1) thisCardPos.x -= (additionalDistance * 1.5f);
                else thisCardPos.x -= additionalDistance;
            }
            else if (i > idx) // 카드 오른쪽 부분
            {
                // 바로 오른쪽일 경우 
                if (i == idx - 1) thisCardPos.x += (additionalDistance * 1.5f);
                else thisCardPos.x += additionalDistance;
            }
               
            thisCardPos.y = thisCardPos.y + -Mathf.Abs(distanceCount) * positionYDegree;
            if (i - halfCount > 0) thisCardRot = thisCardRot + -Mathf.CeilToInt(i - halfCount) * rotationDegree;
            else thisCardRot = thisCardRot + -Mathf.FloorToInt(i - halfCount) * rotationDegree;

            // 임시 보관 (그냥 일시적으로 이동하게 하는 거)
            // unitsList[i].transform.localPosition = thisCardPos;
            // unitsList[i].transform.rotation = Quaternion.Euler(0f, 0f, thisCardRot);

            //unitsList[i].GetComponent<A_HandUnit>().StartCoroutine(TranslateCard(unitsList[i].transform, thisCardPos, thisCardRot));
            StopCoroutine("TranslateCard");
            StartCoroutine(TranslateCard(unitsList[i].transform, thisCardPos, thisCardRot, cardScale));

        }
    }



    public IEnumerator TranslateCard(Transform card, Vector3 targetPos, float thisCardRot, Vector3 targetScale)
    {
        Vector3 startPos = card.localPosition;
        Quaternion startRot = card.rotation;
        Vector3 startScale = card.localScale;

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            card.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            card.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(0f, 0f, thisCardRot), elapsedTime / moveDuration);
            card.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        card.localPosition = targetPos; // 최종 위치 고정
        card.rotation = Quaternion.Euler(0f, 0f, thisCardRot); // 최종 회전 고정
        card.localScale = targetScale;
    }


}
