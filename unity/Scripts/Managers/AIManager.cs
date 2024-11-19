using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIManager : MonoBehaviour
{
    public enum AIDifficulty { Easy = 1, Normal = 2, Hard = 3, Dragon = 4 }

    private AIDifficulty currentDifficulty;
    private int maxDepth;
    private const float RANDOM_DECISION_CHANCE = 0.3f;
    private const int MAX_FIELD_SIZE = 5;

    public void SetDifficulty(AIDifficulty difficulty)
    {
        currentDifficulty = difficulty;
        switch (difficulty)
        {
            case AIDifficulty.Easy: maxDepth = 2; break;
            case AIDifficulty.Normal: maxDepth = 3; break;
            case AIDifficulty.Hard: maxDepth = 4; break;
            case AIDifficulty.Dragon: maxDepth = 5; break;
        }
    }

    #region 랜덤 야매
    public IEnumerator ExecuteAITurn()
    {
        var currentState = GetCurrentGameState();
        if (currentState == null)
        {
            Debug.LogWarning("AI could not retrieve game state. Ending turn.");
            Hub.BattleSceneManager.TurnEnd(false);
            yield break;
        }

        Debug.Log($"AI Turn Start: Mana={currentState.aiMana}, Hand Cards={currentState.aiHand.Count}, Field Units={currentState.aiField.Count}");

        bool turnEnded = false;
        float actionTimer = 0f; // 행동 시간 타이머
        const float MAX_AI_TIME = 5f; // 최대 5초 동안 행동
        bool actionInProgress = false; // 행동 완료 여부 플래그

        while (!turnEnded)
        {
            if (currentState.aiMana <= 0 || actionTimer >= MAX_AI_TIME)
            {
                Debug.LogWarning($"AI Turn Ended due to insufficient mana or timeout (Timer: {actionTimer}s)");
                turnEnded = true;
                break;
            }

            if (actionInProgress)
            {
                // 다른 행동이 완료되지 않았으면 대기
                yield return null;
                continue;
            }

            bool actionTaken = false; // 이번 루프에서 행동 성공 여부

            // 50% 확률로 카드 내기 또는 공격 시도
            bool playCard = Random.value < 0.5f;

            if (playCard)
            {
                // **필드 제한**: 필드에 5장이 넘으면 카드 내기 스킵
                if (Hub.BattleSceneManager.oppBattleField.unitsList.Count >= 5)
                {
                    Debug.LogWarning("AI Field is full. Skipping card play.");
                }
                else
                {
                    // 카드 내기 시도
                    List<int> playableCards = GetPlayableCards(currentState);
                    Debug.Log($"Playable Cards Count: {playableCards.Count}, Mana: {currentState.aiMana}");

                    if (playableCards.Count > 0)
                    {
                        int randomCardIndex = playableCards[Random.Range(0, playableCards.Count)];
                        if (Hub.BattleSceneManager.AIUseHand(randomCardIndex))
                        {
                            // 카드 사용 성공 시 마나 감소
                            int cardCost = currentState.aiHand[randomCardIndex].cost;
                            currentState.aiMana -= cardCost;
                            currentState.aiMana = Mathf.Max(0, currentState.aiMana); // 음수 방지
                            Debug.Log($"AI played card at index {randomCardIndex}. Remaining Mana: {currentState.aiMana}");
                            actionTaken = true;
                        }
                    }
                }
            }
            else
            {
                // 공격 시도
                var aiField = Hub.BattleSceneManager.oppBattleField?.unitsList;
                var playerField = Hub.BattleSceneManager.myBattleField?.unitsList;

                if (aiField != null && aiField.Count > 0)
                {
                    foreach (var attacker in aiField)
                    {
                        var attackerCard = attacker.GetComponentInChildren<A_MinionCardInField>();
                        if (attackerCard == null || attackerCard.isInSleep || attackerCard.IsUsed || attackerCard.Attack <= 0)
                        {
                            Debug.Log($"Skipping Attacker: Sleep={attackerCard?.isInSleep}, Used={attackerCard?.IsUsed}, Attack={attackerCard?.Attack}");
                            continue; // 이미 공격했거나 수면 상태인 유닛은 스킵
                        }

                        int attackerIndex = aiField.IndexOf(attacker);
                        int targetIndex = playerField != null && playerField.Count > 0
                                          ? Random.Range(0, playerField.Count) // 상대 필드 유닛 공격
                                          : -2; // 영웅 공격 (-2)

                        if (ValidateAttackTarget(attackerIndex, targetIndex))
                        {
                            actionInProgress = true; // 행동 진행 중으로 설정

                            // yield return을 try-catch 외부에서 실행
                            yield return Hub.BattleSceneManager.AIUseFieldCardCor(
                                aiField[attackerIndex].transform.GetChild(0).gameObject,
                                targetIndex == -2
                                    ? Hub.BattleSceneManager.playerHero
                                    : playerField[targetIndex].transform.GetChild(0).gameObject
                            );

                            // 행동 결과 처리
                            try
                            {
                                // 공격 성공 시 마나 감소
                                currentState.aiMana--;
                                currentState.aiMana = Mathf.Max(0, currentState.aiMana); // 음수 방지
                                attackerCard.IsUsed = true; // 유닛 상태 업데이트 (한 턴에 한 번만 공격)

                                Debug.Log($"AI attacked: AttackerIndex={attackerIndex}, TargetIndex={targetIndex}, Remaining Mana: {currentState.aiMana}");
                                actionTaken = true;
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError($"Error during attack: {e.Message}");
                            }

                            actionInProgress = false; // 행동 완료
                            break; // 한 번 공격 후 다른 유닛으로 넘어감
                        }
                    }
                }
            }

            // 행동하지 못했으면 타이머 증가
            if (!actionTaken)
            {
                Debug.LogWarning("AI failed to perform any action.");
                actionTimer += 1f; // 타이머 증가
                yield return new WaitForSeconds(1f); // 1초 대기
            }
            else
            {
                // 행동이 성공했으면 타이머 초기화
                actionTimer = 0f;
                yield return new WaitForSeconds(1f); // 행동 간 대기 시간
            }

            // 상태 갱신
            currentState = GetCurrentGameState();
            if (currentState == null)
            {
                Debug.LogWarning("AI lost game state after action. Ending turn.");
                turnEnded = true;
            }
        }

        Debug.Log("AI Turn Ended.");
        Hub.BattleSceneManager.TurnEnd(false);
    }

    private List<int> GetPlayableCards(GameState state)
    {
        var playableCards = new List<int>();
        for (int i = 0; i < state.aiHand.Count; i++)
        {
            if (state.aiHand[i].cost <= state.aiMana) // 카드 비용이 현재 마나보다 작아야 함
            {
                playableCards.Add(i);
            }
        }

        Debug.Log($"Playable Cards: {playableCards.Count}, AI Mana: {state.aiMana}, Hand Count: {state.aiHand.Count}");
        return playableCards;
    }

    private bool ValidateAttackTarget(int attackerIndex, int targetIndex)
    {
        var oppField = Hub.BattleSceneManager?.oppBattleField;
        var myField = Hub.BattleSceneManager?.myBattleField;

        if (oppField == null || myField == null)
        {
            Debug.LogWarning("Battlefield is null. Cannot validate attack target.");
            return false;
        }

        if (attackerIndex < 0 || attackerIndex >= oppField.unitsList.Count)
        {
            Debug.LogWarning($"Invalid attacker index: {attackerIndex}. Attacker count: {oppField.unitsList.Count}");
            return false;
        }

        // 영웅 공격 (-2)
        if (targetIndex == -2) return true;

        // 유닛 공격
        if (targetIndex < 0 || targetIndex >= myField.unitsList.Count)
        {
            Debug.LogWarning($"Invalid target index: {targetIndex}. Target count: {myField.unitsList.Count}");
            return false;
        }

        var attacker = oppField.unitsList[attackerIndex]?.GetComponentInChildren<A_MinionCardInField>();
        var target = myField.unitsList[targetIndex]?.GetComponentInChildren<A_MinionCardInField>();

        if (attacker == null || target == null)
        {
            Debug.LogWarning($"Attacker or target is null. AttackerIndex: {attackerIndex}, TargetIndex: {targetIndex}");
            return false;
        }

        if (attacker.isInSleep || attacker.IsUsed || attacker.Attack <= 0)
        {
            Debug.LogWarning($"Attacker cannot attack. IsInSleep: {attacker.isInSleep}, IsUsed: {attacker.IsUsed}, Attack: {attacker.Attack}");
            return false;
        }

        return true;
    }
    #endregion

    #region ㄹㅇAI
    //public IEnumerator ExecuteAITurn()
    //{
    //    var currentState = GetCurrentGameState();
    //    if (currentState == null)
    //    {
    //        Hub.BattleSceneManager.TurnEnd(false); // 상태가 없으면 턴 종료
    //        yield break;
    //    }

    //    var attackedUnits = new HashSet<int>();
    //    bool turnEnded = false;

    //    // AI 턴 시작 디버깅 로그
    //    Debug.Log($"AI Turn Start: Mana={currentState.aiMana}, OppMana={Hub.BattleSceneManager.OppMana}, Field Units={currentState.aiField.Count}, Hand Cards={currentState.aiHand.Count}");

    //    while (!turnEnded)
    //    {
    //        Debug.Log("AI MANA : " + currentState.aiMana);

    //        // 마나 검증
    //        if (currentState.aiMana <= 0 || Hub.BattleSceneManager.OppMana <= 0)
    //        {
    //            turnEnded = true;
    //            break;
    //        }

    //        // 필드 유닛 유효성 재검증
    //        var validField = ValidateFieldUnits();
    //        if (!validField)
    //        {
    //            Debug.LogWarning("Invalid field state detected");
    //            currentState = GetCurrentGameState(); // 상태 갱신
    //            if (currentState == null)
    //            {
    //                turnEnded = true;
    //                break;
    //            }
    //        }

    //        // 1. 공격 가능한 유닛 검증 후 결정
    //        var possibleAttacks = GetAllPossibleAttacks(currentState, true)
    //            .Where(attack => IsValidAttack(new AIDecision(-1, attack.attackerIndex, attack.targetIndex, 0), attackedUnits))
    //            .ToList();

    //        AIDecision attackDecision;
    //        float attackScore;

    //        if (possibleAttacks.Any())
    //        {
    //            attackDecision = MiniMax(currentState, maxDepth, true, float.MinValue, float.MaxValue);
    //            attackScore = attackDecision.evaluationScore;
    //        }
    //        else
    //        {
    //            attackDecision = new AIDecision(-1, -1, -1, float.MinValue);
    //            attackScore = float.MinValue;
    //        }

    //        // 2. 카드 내기 결정 평가 (마나 체크 포함)
    //        AIDecision playDecision = EvaluatePlayCard(currentState);
    //        float playScore = playDecision.evaluationScore;

    //        // 디버깅: 결정된 행동 로그 출력
    //        Debug.Log($"AI Play Decision: CardIndex={playDecision.playCardIndex}, Score={playDecision.evaluationScore}");
    //        Debug.Log($"AI Attack Decision: AttackerIndex={attackDecision.attackerIndex}, TargetIndex={attackDecision.targetIndex}, Score={attackScore}");

    //        // 실행 가능한 액션 체크
    //        bool canPlayCard = playDecision.playCardIndex != -1 &&
    //                          GetCardCost(playDecision.playCardIndex) <= currentState.aiMana;
    //        bool canAttack = attackDecision.attackerIndex != -1 &&
    //                         IsValidAttack(attackDecision, attackedUnits);

    //        if (!canPlayCard && !canAttack)
    //        {
    //            Debug.LogWarning("AI cannot play cards or attack. Ending turn.");
    //            turnEnded = true;
    //            break;
    //        }

    //        bool actionTaken = false;

    //        // 더 높은 점수의 행동 선택 및 실행
    //        if (attackScore > playScore && canAttack)
    //        {
    //            try
    //            {
    //                if (ValidateAttackTarget(attackDecision.attackerIndex, attackDecision.targetIndex))
    //                {
    //                    Hub.BattleSceneManager.AIUseFieldCard(attackDecision.attackerIndex, attackDecision.targetIndex);
    //                    attackedUnits.Add(attackDecision.attackerIndex);
    //                    Hub.BattleSceneManager.OppMana--;

    //                    // 마나 음수 방지
    //                    Hub.BattleSceneManager.OppMana = Mathf.Max(0, Hub.BattleSceneManager.OppMana);
    //                    actionTaken = true;
    //                    Debug.Log("ATK MANA : " + Hub.BattleSceneManager.OppMana);
    //                }
    //            }
    //            catch (System.Exception e)
    //            {
    //                Debug.LogError($"Attack execution error: {e.Message}");
    //                Hub.BattleSceneManager.OppMana++; // 마나 롤백
    //                turnEnded = true;
    //                break;
    //            }
    //        }
    //        else if (canPlayCard)
    //        {
    //            int cardCost = GetCardCost(playDecision.playCardIndex);
    //            try
    //            {                    
    //                if (cardCost <= Hub.BattleSceneManager.OppMana)
    //                {
    //                    Hub.BattleSceneManager.AIUseHand(playDecision.playCardIndex);
    //                    Hub.BattleSceneManager.OppMana -= cardCost;

    //                    // 마나 음수 방지
    //                    Hub.BattleSceneManager.OppMana = Mathf.Max(0, Hub.BattleSceneManager.OppMana);
    //                    actionTaken = true;
    //                    Debug.Log("CARD MANA : " + Hub.BattleSceneManager.OppMana);
    //                }
    //            }
    //            catch (System.Exception e)
    //            {
    //                Debug.LogError($"Card play error: {e.Message}");
    //                Hub.BattleSceneManager.OppMana += cardCost; // 마나 롤백
    //                turnEnded = true;
    //                break;
    //            }
    //        }

    //        if (!actionTaken)
    //        {
    //            Debug.LogWarning("No action was successfully taken. Ending turn.");
    //            turnEnded = true;
    //            break;
    //        }

    //        yield return new WaitForSeconds(1f);

    //        // 상태 갱신
    //        currentState = GetCurrentGameState();
    //        if (currentState == null)
    //        {
    //            turnEnded = true;
    //            break;
    //        }
    //    }

    //    // 턴 종료 처리
    //    Hub.BattleSceneManager.TurnEnd(false);
    //}


    //public IEnumerator ExecuteAITurn()
    //{
    //    var currentState = GetCurrentGameState();
    //    if (currentState == null)
    //    {
    //        Hub.BattleSceneManager.TurnEnd(false);  // 상태가 없으면 턴 종료
    //        yield break;
    //    }

    //    var attackedUnits = new HashSet<int>();
    //    bool turnEnded = false;

    //    while (!turnEnded)
    //    {
    //        Debug.Log("AI MANA : " + currentState.aiMana);

    //        // 마나 검증
    //        if (currentState.aiMana <= 0 || Hub.BattleSceneManager.OppMana <= 0)
    //        {
    //            turnEnded = true;
    //            break;
    //        }

    //        // 필드 유닛 유효성 재검증
    //        var validField = ValidateFieldUnits();
    //        if (!validField)
    //        {
    //            Debug.LogWarning("Invalid field state detected");
    //            currentState = GetCurrentGameState(); // 상태 갱신
    //            if (currentState == null)
    //            {
    //                turnEnded = true;
    //                break;
    //            }
    //        }

    //        // 1. 공격 가능한 유닛 검증 후 결정
    //        var possibleAttacks = GetAllPossibleAttacks(currentState, true)
    //            .Where(attack => IsValidAttack(new AIDecision(-1, attack.attackerIndex, attack.targetIndex, 0), attackedUnits))
    //            .ToList();

    //        AIDecision attackDecision;
    //        float attackScore;

    //        if (possibleAttacks.Any())
    //        {
    //            attackDecision = MiniMax(currentState, maxDepth, true, float.MinValue, float.MaxValue);
    //            attackScore = attackDecision.evaluationScore;
    //        }
    //        else
    //        {
    //            attackDecision = new AIDecision(-1, -1, -1, float.MinValue);
    //            attackScore = float.MinValue;
    //        }

    //        // 2. 카드 내기 결정 평가 (마나 체크 포함)
    //        AIDecision playDecision = EvaluatePlayCard(currentState);
    //        float playScore = playDecision.evaluationScore;

    //        // 실행 가능한 액션 체크
    //        bool canPlayCard = playDecision.playCardIndex != -1 &&
    //                          GetCardCost(playDecision.playCardIndex) <= currentState.aiMana;
    //        bool canAttack = attackDecision.attackerIndex != -1 &&
    //                        IsValidAttack(attackDecision, attackedUnits);

    //        if (!canPlayCard && !canAttack)
    //        {
    //            turnEnded = true;
    //            break;
    //        }

    //        bool actionTaken = false;

    //        // 더 높은 점수의 행동 선택 및 실행
    //        if (attackScore > playScore && canAttack)
    //        {
    //            try
    //            {
    //                if (ValidateAttackTarget(attackDecision.attackerIndex, attackDecision.targetIndex))
    //                {
    //                    Hub.BattleSceneManager.AIUseFieldCard(attackDecision.attackerIndex, attackDecision.targetIndex);
    //                    attackedUnits.Add(attackDecision.attackerIndex);
    //                    Hub.BattleSceneManager.OppMana--;
    //                    Hub.BattleSceneManager.OppMana = Mathf.Max(0, Hub.BattleSceneManager.OppMana);
    //                    actionTaken = true;
    //                    Debug.Log("ATK MANA : " + Hub.BattleSceneManager.OppMana);
    //                }
    //            }
    //            catch (System.Exception e)
    //            {
    //                Debug.LogError($"Attack execution error: {e.Message}");
    //                turnEnded = true;
    //                break;
    //            }
    //        }
    //        else if (canPlayCard)
    //        {
    //            try
    //            {
    //                int cardCost = GetCardCost(playDecision.playCardIndex);
    //                if (cardCost <= Hub.BattleSceneManager.OppMana)
    //                {
    //                    Hub.BattleSceneManager.AIUseHand(playDecision.playCardIndex);
    //                    Hub.BattleSceneManager.OppMana -= cardCost;
    //                    Hub.BattleSceneManager.OppMana = Mathf.Max(0, Hub.BattleSceneManager.OppMana);
    //                    actionTaken = true;
    //                    Debug.Log("CARD MANA : " + Hub.BattleSceneManager.OppMana);
    //                }
    //            }
    //            catch (System.Exception e)
    //            {
    //                Debug.LogError($"Card play error: {e.Message}");
    //                turnEnded = true;
    //                break;
    //            }
    //        }

    //        if (!actionTaken)
    //        {
    //            turnEnded = true;
    //            break;
    //        }

    //        yield return new WaitForSeconds(1f);

    //        // 상태 갱신
    //        currentState = GetCurrentGameState();
    //        if (currentState == null)
    //        {
    //            turnEnded = true;
    //            break;
    //        }

    //        Debug.Log($"AI Play Decision: CardIndex={playDecision.playCardIndex}, Score={playDecision.evaluationScore}");
    //        Debug.Log($"AI Attack Decision: AttackerIndex={attackDecision.attackerIndex}, TargetIndex={attackDecision.targetIndex}, Score={attackScore}");
    //    }

    //    // 턴 종료 처리
    //    Hub.BattleSceneManager.TurnEnd(false);
    //}

    private bool ValidateFieldUnits()
    {
        if (Hub.BattleSceneManager?.oppBattleField?.unitsList == null)
            return false;

        // 필드 유닛 유효성 검사
        for (int i = Hub.BattleSceneManager.oppBattleField.unitsList.Count - 1; i >= 0; i--)
        {
            var unit = Hub.BattleSceneManager.oppBattleField.unitsList[i];
            if (unit == null || unit.GetComponentInChildren<A_MinionCardInField>() == null)
            {
                Hub.BattleSceneManager.oppBattleField.unitsList.RemoveAt(i);
            }
        }

        return true;
    }

    //private bool ValidateAttackTarget(int attackerIndex, int targetIndex)
    //{
    //    var oppField = Hub.BattleSceneManager?.oppBattleField;
    //    var myField = Hub.BattleSceneManager?.myBattleField;

    //    if (oppField == null || myField == null) return false;
    //    if (attackerIndex < 0 || attackerIndex >= oppField.unitsList.Count) return false;

    //    // 영웅 공격인 경우
    //    if (targetIndex == -2) return true;

    //    // 필드 유닛 공격인 경우
    //    if (targetIndex < 0 || targetIndex >= myField.unitsList.Count) return false;

    //    var attacker = oppField.unitsList[attackerIndex]?.GetComponentInChildren<A_MinionCardInField>();
    //    var target = myField.unitsList[targetIndex]?.GetComponentInChildren<A_MinionCardInField>();

    //    return attacker != null && target != null;
    //}

    private AIDecision MiniMax(GameState state, int depth, bool isMaximizing, float alpha, float beta)
    {
        if (depth == 0 || IsGameOver(state))
        {
            return new AIDecision(-1, -1, -1, EvaluateState(state));
        }

        if (isMaximizing)
        {
            float maxEval = float.MinValue;
            AIDecision bestMove = new AIDecision(-1, -1, -1, maxEval);

            // 카드 내기 시도
            List<int> playableCards = GetPlayableCards(state);
            foreach (int cardIndex in playableCards)
            {
                GameState newState = SimulatePlayCard(state.Clone(), cardIndex);
                AIDecision eval = MiniMax(newState, depth - 1, false, alpha, beta);

                if (eval.evaluationScore > maxEval)
                {
                    maxEval = eval.evaluationScore;
                    bestMove = new AIDecision(cardIndex, -1, -1, maxEval);
                }

                alpha = Mathf.Max(alpha, maxEval);
                if (beta <= alpha) break;
            }

            // 공격 시도
            List<AttackMove> possibleAttacks = GetAllPossibleAttacks(state);
            foreach (var attack in possibleAttacks)
            {
                GameState newState = SimulateAttack(state.Clone(), attack.attackerIndex, attack.targetIndex);
                AIDecision eval = MiniMax(newState, depth - 1, false, alpha, beta);

                if (eval.evaluationScore > maxEval)
                {
                    maxEval = eval.evaluationScore;
                    bestMove = new AIDecision(-1, attack.attackerIndex, attack.targetIndex, maxEval);
                }

                alpha = Mathf.Max(alpha, maxEval);
                if (beta <= alpha) break;
            }

            return bestMove;
        }
        else
        {
            float minEval = float.MaxValue;
            AIDecision worstMove = new AIDecision(-1, -1, -1, minEval);

            // 플레이어 턴 시뮬레이션
            List<AttackMove> playerMoves = GetAllPossibleAttacks(state, false);
            foreach (var move in playerMoves)
            {
                GameState newState = SimulateAttack(state.Clone(), move.attackerIndex, move.targetIndex, false);
                AIDecision eval = MiniMax(newState, depth - 1, true, alpha, beta);

                if (eval.evaluationScore < minEval)
                {
                    minEval = eval.evaluationScore;
                    worstMove = new AIDecision(-1, move.attackerIndex, move.targetIndex, minEval);
                }

                beta = Mathf.Min(beta, minEval);
                if (beta <= alpha) break;
            }

            return worstMove;
        }
    }

    private GameState GetCurrentGameState()
    {
        if (Hub.BattleSceneManager == null ||
            Hub.BattleSceneManager.oppField == null ||
            Hub.BattleSceneManager.myField == null ||
            Hub.BattleSceneManager.oppHand == null)
            return null;

        var state = new GameState
        {
            aiField = new List<CardInfo>(),
            playerField = new List<CardInfo>(),
            aiHand = new List<CardInfo>(),
            aiHealth = Hub.BattleSceneManager.OppHP,
            playerHealth = Hub.ProgressManager.CurrentHealth,
            aiMana = Hub.BattleSceneManager.OppMana
        };

        var oppField = Hub.BattleSceneManager.oppField.GetComponent<A_BattleField>();
        var myField = Hub.BattleSceneManager.myField.GetComponent<A_BattleField>();

        if (oppField?.unitsList != null)
        {
            foreach (var card in oppField.unitsList)
            {
                if (card == null) continue;
                var fieldCard = card.GetComponentInChildren<A_MinionCardInField>();
                if (fieldCard != null)
                    state.aiField.Add(CardInfo.FromFieldCard(fieldCard));
            }
        }

        if (myField?.unitsList != null)
        {
            foreach (var card in myField.unitsList)
            {
                if (card == null) continue;
                var fieldCard = card.GetComponentInChildren<A_MinionCardInField>();
                if (fieldCard != null)
                    state.playerField.Add(CardInfo.FromFieldCard(fieldCard));
            }
        }

        if (Hub.BattleSceneManager.oppHandList != null)
        {
            foreach (var card in Hub.BattleSceneManager.oppHandList)
            {
                if (card == null) continue;
                var minionCard = card.GetComponent<A_MinionCard>();
                if (minionCard != null)
                    state.aiHand.Add(CardInfo.FromMinionCard(minionCard));
            }
        }

        return state;
    }

    private bool IsGameOver(GameState state)
    {
        return state.playerHealth <= 0 || state.aiHealth <= 0;
    }

    //private List<int> GetPlayableCards(GameState state)
    //{
    //    var playableCards = new List<int>();
    //    for (int i = 0; i < state.aiHand.Count; i++)
    //    {
    //        if (state.aiHand[i].cost <= state.aiMana && state.aiField.Count < MAX_FIELD_SIZE)
    //        {
    //            playableCards.Add(i);
    //        }
    //    }
    //    return playableCards;
    //}
    //private List<int> GetPlayableCards(GameState state)
    //{
    //    var playableCards = new List<int>();
    //    for (int i = 0; i < state.aiHand.Count; i++)
    //    {
    //        // 필드가 가득 찼더라도 카드를 낼 수 있도록 필드 크기 조건 제거
    //        if (state.aiHand[i].cost <= state.aiMana)
    //        {
    //            playableCards.Add(i);
    //        }
    //    }
    //    return playableCards;
    //}

    private bool IsValidPlay(AIDecision decision, GameState state)
    {
        if (Hub.BattleSceneManager?.oppHandList == null ||
            decision.playCardIndex >= Hub.BattleSceneManager.oppHandList.Count)
            return false;

        var cardObj = Hub.BattleSceneManager.oppHandList[decision.playCardIndex];
        if (cardObj == null) return false;

        var card = cardObj.GetComponent<A_MinionCard>();
        return card != null &&
               card.cost <= state.aiMana &&
               state.aiField.Count < MAX_FIELD_SIZE;
    }

    private bool IsValidAttack(AIDecision decision, HashSet<int> attackedUnits)
    {
        if (attackedUnits.Contains(decision.attackerIndex))
            return false;

        if (Hub.BattleSceneManager?.oppBattleField?.unitsList == null ||
            Hub.BattleSceneManager?.myBattleField?.unitsList == null)
            return false;

        if (decision.attackerIndex < 0 ||
            decision.attackerIndex >= Hub.BattleSceneManager.oppBattleField.unitsList.Count)
            return false;

        if (decision.targetIndex >= Hub.BattleSceneManager.myBattleField.unitsList.Count)
            return false;

        var attacker = Hub.BattleSceneManager.oppBattleField.unitsList[decision.attackerIndex];
        if (attacker == null) return false;

        var attackerCard = attacker.GetComponentInChildren<A_MinionCardInField>();
        if (attackerCard == null || attackerCard.isInSleep || attackerCard.IsUsed)
            return false;

        return true;
    }

    private float EvaluateState(GameState state)
    {
        if (state.playerHealth <= 0) return float.MaxValue;
        if (state.aiHealth <= 0) return float.MinValue;

        float score = 0;

        // 생명력 가중치
        float healthWeight = 5.0f;
        score += (state.aiHealth / (float)state.playerHealth) * healthWeight;

        // 필드 지배력
        float fieldControlWeight = 3.0f;
        float aiFieldPower = CalculateFieldPower(state.aiField);
        float playerFieldPower = CalculateFieldPower(state.playerField);
        score += (aiFieldPower - playerFieldPower) * fieldControlWeight;

        // 카드 어드밴티지
        float cardAdvantageWeight = 1.5f;
        score += (state.aiHand.Count - state.playerField.Count) * cardAdvantageWeight;

        // 전략적 보너스
        score += CalculateStrategicBonus(state);

        // 난이도 보정
        switch (currentDifficulty)
        {
            case AIDifficulty.Easy: score *= 0.8f; break;
            case AIDifficulty.Normal: score *= 1.0f; break;
            case AIDifficulty.Hard: score *= 1.1f; break;
            case AIDifficulty.Dragon: score *= 1.2f; break;
        }

        return score;
    }

    private float CalculateFieldPower(List<CardInfo> field)
    {
        float power = 0;
        foreach (var card in field)
        {
            power += card.attack * 1.8f + card.health;
            if (card.isTaunt) power += card.health * 0.8f;
            if (card.isShield) power += card.attack * 0.7f;
            if (card.canAttack) power += 2f;
            if (card.attack >= 4) power += 3f;
            if (card.health >= 5) power += 2f;
        }
        return power;
    }

    private float CalculateStrategicBonus(GameState state)
    {
        float bonus = 0;

        bool aiHasTaunt = state.aiField.Any(c => c.isTaunt);
        bool playerHasTaunt = state.playerField.Any(c => c.isTaunt);

        if (aiHasTaunt && !playerHasTaunt) bonus += 15;
        if (!aiHasTaunt && playerHasTaunt) bonus -= 20;

        bool hasLethalThreat = state.aiField.Any(c => c.attack >= state.playerHealth);
        if (hasLethalThreat) bonus += 30;

        if (state.aiField.Count > state.playerField.Count + 1) bonus += 8;

        return bonus;
    }

    private List<AttackMove> GetAllPossibleAttacks(GameState state, bool isAI = true)
    {
        var moves = new List<AttackMove>();
        if (state == null) return moves;

        var attackers = isAI ? state.aiField : state.playerField;
        var defenders = isAI ? state.playerField : state.aiField;

        if (attackers == null || defenders == null)
            return moves;

        foreach (var attacker in attackers.Where(c => c?.canAttack == true))
        {
            int attackerIdx = attackers.IndexOf(attacker);
            if (attackerIdx == -1) continue;

            bool hasTaunt = defenders.Any(d => d?.isTaunt == true);

            // 도발이 없으면 영웅 공격 가능
            if (!hasTaunt)
                moves.Add(new AttackMove(attackerIdx, -2));

            // 하수인 공격
            for (int i = 0; i < defenders.Count; i++)
            {
                if (!hasTaunt || defenders[i]?.isTaunt == true)
                    moves.Add(new AttackMove(attackerIdx, i));
            }
        }

        // 공격 우선순위 정렬
        moves.Sort((a, b) => {
            float evalA = EvaluateAttackValue(a, state);
            float evalB = EvaluateAttackValue(b, state);
            return evalB.CompareTo(evalA);
        });

        return moves;
    }

    private float EvaluateAttackValue(AttackMove move, GameState state)
    {
        if (state.aiField == null || move.attackerIndex >= state.aiField.Count)
            return float.MinValue;

        // 영웅 공격
        if (move.targetIndex == -2)
        {
            // 치명타 상황
            if (state.aiField[move.attackerIndex].attack >= state.playerHealth)
                return float.MaxValue;
            return state.aiField[move.attackerIndex].attack * 3;
        }

        // 하수인 공격
        if (state.playerField == null || move.targetIndex >= state.playerField.Count)
            return float.MinValue;

        var attacker = state.aiField[move.attackerIndex];
        var defender = state.playerField[move.targetIndex];
        float value = 0;

        // 상대 하수인 제거 가능
        if (attacker.attack >= defender.health)
        {
            value += defender.attack * 2.5f + defender.health;
            if (defender.isTaunt) value += 8; // 도발 제거 보너스
            if (defender.isShield) value += 5; // 보호막 제거 보너스
        }

        // 안전한 교환이면 추가 점수
        if (attacker.health > defender.attack || attacker.isShield)
            value += 3;

        return value;
    }

    private AIDecision EvaluatePlayCard(GameState state)
    {
        var playableCards = GetPlayableCards(state);
        float maxScore = float.MinValue;
        int bestCardIndex = -1;

        foreach (int cardIndex in playableCards)
        {
            GameState newState = SimulatePlayCard(state.Clone(), cardIndex);
            float score = EvaluateState(newState);

            if (score > maxScore)
            {
                maxScore = score;
                bestCardIndex = cardIndex;
            }
        }

        return new AIDecision(bestCardIndex, -1, -1, maxScore);
    }

    private int GetCardCost(int cardIndex)
    {
        if (Hub.BattleSceneManager?.oppHandList == null ||
            cardIndex >= Hub.BattleSceneManager.oppHandList.Count)
            return 999;

        var card = Hub.BattleSceneManager.oppHandList[cardIndex]?.GetComponent<A_MinionCard>();
        return card?.cost ?? 999;
    }

    private GameState SimulatePlayCard(GameState state, int handIndex)
    {
        var card = state.aiHand[handIndex];
        state.aiMana -= card.cost;
        state.aiField.Add(card);
        state.aiHand.RemoveAt(handIndex);
        return state;
    }

    private GameState SimulateAttack(GameState state, int attackerIndex, int targetIndex, bool isAI = true)
    {
        if (targetIndex == -2) // 영웅 공격
        {
            if (isAI)
                state.playerHealth -= state.aiField[attackerIndex].attack;
            else
                state.aiHealth -= state.playerField[attackerIndex].attack;
            return state;
        }

        var attacker = isAI ? state.aiField[attackerIndex] : state.playerField[attackerIndex];
        var defender = isAI ? state.playerField[targetIndex] : state.aiField[targetIndex];

        // 보호막 처리
        if (defender.isShield)
            defender.isShield = false;
        else
            defender.health -= attacker.attack;

        if (attacker.isShield)
            attacker.isShield = false;
        else
            attacker.health -= defender.attack;

        // 죽은 하수인 제거
        if (isAI)
        {
            state.aiField.RemoveAll(c => c.health <= 0);
            state.playerField.RemoveAll(c => c.health <= 0);
        }
        else
        {
            state.playerField.RemoveAll(c => c.health <= 0);
            state.aiField.RemoveAll(c => c.health <= 0);
        }
        return state;
    }
    #endregion

    #region Data Classes
    public class GameState
    {
        public List<CardInfo> aiField = new List<CardInfo>();
        public List<CardInfo> playerField = new List<CardInfo>();
        public List<CardInfo> aiHand = new List<CardInfo>();
        public int aiHealth;
        public int playerHealth;
        public int aiMana;

        public GameState Clone()
        {
            return new GameState
            {
                aiField = new List<CardInfo>(aiField.Select(c => c.Clone())),
                playerField = new List<CardInfo>(playerField.Select(c => c.Clone())),
                aiHand = new List<CardInfo>(aiHand.Select(c => c.Clone())),
                aiHealth = aiHealth,
                playerHealth = playerHealth,
                aiMana = aiMana
            };
        }
    }

    public class CardInfo
    {
        public int attack;
        public int health;
        public int cost;
        public bool isTaunt;
        public bool isShield;
        public bool canAttack;

        public CardInfo Clone()
        {
            return new CardInfo
            {
                attack = attack,
                health = health,
                cost = cost,
                isTaunt = isTaunt,
                isShield = isShield,
                canAttack = canAttack
            };
        }

        public static CardInfo FromMinionCard(A_MinionCard card)
        {
            return new CardInfo
            {
                attack = card.attack,
                health = card.health,
                cost = card.cost,
                isTaunt = card.abilityButtons[0],
                isShield = card.abilityButtons[2],
                canAttack = false
            };
        }

        public static CardInfo FromFieldCard(A_MinionCardInField card)
        {
            return new CardInfo
            {
                attack = card.Attack,
                health = card.Health,
                cost = 0,
                isTaunt = card.IsTaunt,
                isShield = card.IsShield,
                canAttack = !card.isInSleep && !card.IsUsed
            };
        }
    }

    public struct AIDecision
    {
        public int playCardIndex;
        public int attackerIndex;
        public int targetIndex;
        public float evaluationScore;

        public AIDecision(int play, int attacker, int target, float score)
        {
            playCardIndex = play;
            attackerIndex = attacker;
            targetIndex = target;
            evaluationScore = score;
        }
    }

    private struct AttackMove
    {
        public int attackerIndex;
        public int targetIndex;

        public AttackMove(int attacker, int target)
        {
            attackerIndex = attacker;
            targetIndex = target;
        }
    }
    #endregion
}