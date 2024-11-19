using UnityEngine;

public class AbilityButtons : MonoBehaviour
{
    public bool[] abilityButtons = new bool[2];
    public string[] abilitylabel = new string[2] { "도발", "전투의 함성" };

    // 버튼 클릭 시 호출될 메서드
    public void OnBoolButtonClicked(int index)
    {
        // 배열의 해당 인덱스의 bool 값 토글
        abilityButtons[index] = !abilityButtons[index];

        // 필요한 동작 수행
        if (abilityButtons[index])
        {
            Debug.Log("켜짐: " + abilitylabel[index]);
        }
        else
        {
            Debug.Log("꺼짐: " + abilitylabel[index]);
        }
    }
}
