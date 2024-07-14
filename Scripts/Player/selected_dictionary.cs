using System.Collections.Generic;
using UnityEngine;

public class selected_dictionary : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> allArmyTable = new Dictionary<int, GameObject>();
    public UIManager UIManager;
    private int UnitCount = 0; // Число выделенных юнитов
    private int AssignedUnitNumbers = 0; // Выдаем номер юнита по мере добавления его к выделению

    // 25 feb 24
    public Dictionary<int, int> selectedCount = new Dictionary<int, int>();
    public Dictionary<int, GameObject> playableUnits = new Dictionary<int, GameObject>();
    private float timer=0;

    private void Start()
    {

    }

    private void Update()
    {
        timer = timer + Time.deltaTime;
        if (timer > 1)
        {
           
            UpdateSelected(); // Тут делается все, что надо обновлять регулярно
            timer = 0;
        }

    }


    public int UnitsExistInArmy()
    {
        int num = 0;
        foreach (KeyValuePair<int, GameObject> item in allArmyTable)
        {
            if (item.Value.GetComponent<PlayerStateManager>().UnitNumber != 0)
            {
                num++;
            }
        }
        return num;
    }

    /// <summary>
    /// При смене управляемого юнита выдает следующего по списку instance ID
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public GameObject GetNextPlayable(GameObject givenGO, int side)
    {
        GameObject[] allUnits;
        allUnits = GameObject.FindGameObjectsWithTag("SelectableUnit");
        playableUnits.Clear();  //new Dictionary<int, GameObject>();

        // Прошлись по всем юнитам на карте. Если играбелен и правильной стороны, то добавили в словарь playableUnits
        foreach (GameObject go in allUnits)
        {
            if (go.GetComponent<PlayerStateManager>() != null)
            {
                 if (go.GetComponent<PlayerStateManager>().IsPlayable && !go.GetComponent<PlayerStateManager>().isDead && go.GetComponent<PlayerStateManager>().Side == side)
                 {
                    playableUnits.Add(go.GetInstanceID(), go);
                 }
             }
        }

        int counter = 0;
        bool getThisOne = false;    // мы нашли тот объект в списке, который запрашивался - следующий в списке и нужно отдать
        GameObject returnThis;
        foreach (KeyValuePair<int, GameObject> playCandidates in playableUnits)
        {
            counter++;

            if (getThisOne) // уже нашли наш объект, с которого меняем, и возвращаем следующий
            {
                returnThis = (GameObject)playCandidates.Value;
                return returnThis;
            }
            if (playCandidates.Key == givenGO.GetInstanceID() // перебирая юниты в списке кандидатов, нашли объект, с которого меняем
                && counter < playableUnits.Count) getThisOne = true; // и он не последний объект в списке. Значит, возвращаем следующий объект из списка (след. итерация)
        }

        // Объект с которого меняем - последний в списке. Возвращаем первый объект списка.
        foreach (KeyValuePair<int, GameObject> playCandidates in playableUnits)
        {
            returnThis = (GameObject)playCandidates.Value;
            return returnThis;
        }

        // No other Playable unit
        return givenGO;

    }


        /// <summary>
        /// Развыделить всех кроме этого юнита
        /// </summary>
        /// <param name="UnitIndex"></param>
        public void SelectOnlyMe(int UnitIndex)
    {
        int unitID=0;
        GameObject go = this.gameObject;
        foreach (KeyValuePair<int, int> units in selectedCount)
        {
            if (units.Value != 0)
            {
                if (units.Value == UnitIndex) unitID = units.Key;
            }
        }

        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                if (pair.Key == unitID)  go = (GameObject)pair.Value;

            }
        }

        deselectAll();

        addSelected(go);
    }





    public void SelectOnlyMeFromArmy(int UnitIndex)
    {
        //int unitID = 0;
        GameObject go = this.gameObject;

        foreach (KeyValuePair<int, GameObject> pair in allArmyTable)
        {
            if (pair.Value != null)
            {
                if (pair.Value.GetComponent<PlayerStateManager>().UnitNumber == UnitIndex)
                {
                    go = (GameObject)pair.Value;
                }
                
                

            }
        }
        deselectAll();
        addSelected(go);
    }


    /// <summary>
    /// Есть ли в списке юнитов армии юнит с таким порядковым номером? 
    /// </summary>
    /// <param name="UnitIndex"></param>
    /// <returns></returns>
    public bool ExistsUnitInArmy(int UnitIndex)
    {

        foreach (KeyValuePair<int, GameObject> units in allArmyTable)
        {
            if (units.Value.GetComponent<PlayerStateManager>().UnitNumber != 0)
            {


                // Debug.Log($"Looking for unit number:" + UnitIndex + " | found ID: " + units.Value);
                if (units.Value.GetComponent<PlayerStateManager>().UnitNumber == UnitIndex) return true;

            }
        }
        return false;

    }

    public void CheckDeadInArmy(int UnitIndex)
    {
        int UID=0;

        foreach (KeyValuePair<int, GameObject> units in selectedTable)
        {
            if (units.Value.GetComponent<PlayerStateManager>().UnitNumber != 0)
            {


                // Debug.Log($"Looking for unit number:" + UnitIndex + " | found ID: " + units.Value);
                if (units.Value.GetComponent<PlayerStateManager>().isDead)
                {
                
                        
                    UID = units.Key;
                        //Debug.Log("ERROR HERE");

                }

            }
        }

        if (UID != 0)
        {
            deselect(UID);
        }


    }


    public void RemoveSelectionFromArmy(int UnitIndex)
    {
        //int unitID = 0;
        GameObject go = this.gameObject;


        foreach (KeyValuePair<int, GameObject> units in allArmyTable)
        {
            if (units.Value.GetComponent<PlayerStateManager>().UnitNumber != 0)
            {


                if (units.Value.GetComponent<PlayerStateManager>().UnitNumber == UnitIndex)
                {

                    //go = (GameObject)units.Value;
                    deselect(units.Key);

                }

            }
        }
        //deselectAll();
        //addSelected(go);
    }


    public void AddSelectionFromArmy(int UnitIndex)
    {
        //int unitID = 0;
        GameObject go = this.gameObject;


        foreach (KeyValuePair<int, GameObject> units in allArmyTable)
        {
            if (units.Value.GetComponent<PlayerStateManager>().UnitNumber != 0)
            {


                if (units.Value.GetComponent<PlayerStateManager>().UnitNumber == UnitIndex)
                {

                    go = (GameObject)units.Value;
                    //deselect(units.Key);

                }

            }
        }
        //deselectAll();
        addSelected(go);
    }



    public bool UnitIsSelected(int UnitIndex)
    {
        //int unitID = 0;
        //GameObject go = this.gameObject;


        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                if (pair.Value.GetComponent<PlayerStateManager>().UnitNumber == UnitIndex)
                {
                    return true;
                } 

            }
        }
        return false;

    }

    /// <summary>
    /// Есть ли в нашем списке юнит с таким порядковым номером? Имеется в виду общее число юнитов.
    /// </summary>
    /// <param name="UnitIndex"></param>
    /// <returns></returns>
    public bool ExistsUnit(int UnitIndex)
    {

        foreach (KeyValuePair<int, int> units in selectedCount)
        {
                if (units.Value != 0)
               {


           // Debug.Log($"Looking for unit number:" + UnitIndex + " | found ID: " + units.Value);
            if (units.Value == UnitIndex) return true;

            }
        }
        return false;

    }





    public int GetUnitHealth(int UnitIndex)
    {
        foreach (KeyValuePair<int, int> units in selectedCount)
        {
            if (units.Value != 0)
            {

                if (units.Value == UnitIndex)
                {
                    foreach (KeyValuePair<int, GameObject> pair in selectedTable)
                    {
                        if (pair.Key == units.Key)
                        {
                            return selectedTable[pair.Key].GetComponent<PlayerStateManager>().HitPoints;
                        }
                    }
                }

            }
        }
        return 0;
    }

    public int GetUnitHealthArmy(int UnitIndex)
    {
        foreach (KeyValuePair<int, GameObject> units in allArmyTable)
        {
                if (units.Value.GetComponent<PlayerStateManager>().UnitNumber == UnitIndex)
                {
                return units.Value.GetComponent<PlayerStateManager>().HitPoints;
            }
        }
        return 0;
    }

    /// <summary>
    /// Не используется, так как теперь юниту назначается номер при добавлении к выделению, в порядке добавления, а не в порядке ID юнита
    /// </summary>
    /// <param name="UnitIndex"></param>
    /// <returns></returns>
    public int GetUnitNumber(int UnitIndex)
    {
        foreach (KeyValuePair<int, int> units in selectedCount)
        {
            if (units.Value != 0)
            {

                if (units.Value == UnitIndex)
                {
                    foreach (KeyValuePair<int, GameObject> pair in selectedTable)
                    {
                        if (pair.Key == units.Key)
                        {
                            return selectedTable[pair.Key].GetComponent<PlayerStateManager>().UnitNumber;
                        }
                    }
                }

            }
        }
        return 0;
    }

    public Sprite GetPortrait (int UnitIndex)
    {
        //Sprite getImg = new Sprite;

        foreach (KeyValuePair<int, int> units in selectedCount)
        {
            if (units.Value != 0)
            {


                // Debug.Log($"Looking for unit number:" + UnitIndex + " | found ID: " + units.Value);
                if (units.Value == UnitIndex)
                {

                    foreach (KeyValuePair<int, GameObject> pair in selectedTable)
                    {
                        if (pair.Key == units.Key)
                        {

                            //getImg.sprite = selectedTable[pair.Key].GetComponent<PlayerController>().charPortrait;
                            //return getImg;
                            return selectedTable[pair.Key].GetComponent<PlayerStateManager>().charPortrait;


                        }
                    }
                }

            }
        }
        return null;

    }



    public Sprite GetPortraitInArmy(int UnitIndex)
    {
        //Sprite getImg = new Sprite;

        foreach (KeyValuePair<int, GameObject> units in allArmyTable)
        {
          //  if (units.Value.GetComponent<PlayerStateManager>().UnitNumber != 0)
           // {


                // Debug.Log($"Looking for unit number:" + UnitIndex + " | found ID: " + units.Value);
                if (units.Value.GetComponent<PlayerStateManager>().UnitNumber == UnitIndex)
                {
                return units.Value.GetComponent<PlayerStateManager>().charPortrait;

                /*
                    foreach (KeyValuePair<int, GameObject> pair in selectedTable)
                    {
                        if (pair.Key == units.Key)
                        {

                            //getImg.sprite = selectedTable[pair.Key].GetComponent<PlayerController>().charPortrait;
                            //return getImg;
                            return selectedTable[pair.Key].GetComponent<PlayerStateManager>().charPortrait;


                        }
                    }
                */
            }

           // }
        }
        return null;

    }

    /// <summary>
    /// Заполняем список выделенных юнитов ID - порядковый номер юнита
    /// </summary>
    public void FormUnitIndexTable()
    {
        int UnitIndex=0;
        selectedCount.Clear();
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                UnitIndex++;

                selectedCount.Add(pair.Key, UnitIndex);
                //Debug.Log($"Unit in list:" + UnitIndex + "with ID: " + pair.Key);

            }
        }
    }


    /// <summary>
    /// Добавляем GameObject в список выделенных юнитов ID - GameObject
    /// </summary>
    /// <param name="go"></param>
    public void addSelected(GameObject go)
    {
        int id = go.GetInstanceID();


        if (!go.GetComponent<PlayerStateManager>().isDead)
        {

            if (!(selectedTable.ContainsKey(id)))
            {
                selectedTable.Add(id, go);
                //go.AddComponent<selection_component>();
                go.GetComponent<PlayerStateManager>().Select();
                //Debug.Log("Added " + id + " to selected dict");
                UnitCount += 1;

                // Назначаем номер юниту после выделения
                //if (go.GetComponent<PlayerStateManager>().UnitNumber ==0)
                //{
                //   AssignedUnitNumbers++;
                //   go.GetComponent<PlayerStateManager>().UnitNumber = AssignedUnitNumbers;
                //}
            }

            if (!(allArmyTable.ContainsKey(id)))
            {


                allArmyTable.Add(id, go);
                //go.AddComponent<selection_component>();
                //go.GetComponent<PlayerStateManager>().Select();
                //Debug.Log("Added " + id + " to selected dict");
                AssignedUnitNumbers++;

                go.GetComponent<PlayerStateManager>().UnitNumber = AssignedUnitNumbers;


                // Назначаем номер юниту после выделения
                /*
                if (go.GetComponent<PlayerStateManager>().UnitNumber == 0)
                {
                    AssignedUnitNumbers++;
                    go.GetComponent<PlayerStateManager>().UnitNumber = AssignedUnitNumbers;
                }
                */
            }


            FormUnitIndexTable(); // добавили порядковые номера юнитов
            UIManager.UpdateUnitIcons(UnitCount); // перерисовали иконки выделенных юнитов
        }
    }

    public void UpdateSelected()
    {

        //RemoveDead();

        FormUnitIndexTable(); // добавили порядковые номера юнитов
        UIManager.UpdateUnitIcons(UnitCount); // перерисовали иконки выделенных юнитов
        //AssignUnitsRawNumber(); // назначить всем юнитам порядковый номер
        
    }


    public void RemoveDead()
    {

        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {

                if (pair.Value.GetComponent<PlayerStateManager>().isDead)
                {
                    deselect(pair.Key);
                    //Debug.Log("ERROR HERE");
                }


            }
        }

    }
    /*
    public void AssignUnitsRawNumber()
    {
        int new_unit_number;
        GameObject[] allUnits;
        allUnits = GameObject.FindGameObjectsWithTag("SelectableUnit");
        


        for (int team = 0; team < 10; team++) 
        {
            new_unit_number=0;

            foreach (GameObject go in allUnits)
            {

                if (go.GetComponent<PlayerStateManager>() != null)
                {

                    if (go.GetComponent<PlayerStateManager>().side == team) //&& !go.GetComponent<PlayerStateManager>().isDead)
                    {
                        new_unit_number++;
                        //Debug.Log($"We have Unit:" + go.GetInstanceID());
                        //playableUnits.Add(go.GetInstanceID(), go);
                        go.GetComponent<PlayerStateManager>().UnitNumber = new_unit_number;
                    }
                }
            }
        }
    }
    */

    /// <summary>
    /// Развыделяем юнит из списка выделенных по ID
    /// </summary>
    /// <param name="id"></param>
    public void deselect(int id)
    {
        //Destroy(selectedTable[id].GetComponent<selection_component>());
        selectedTable[id].GetComponent<PlayerStateManager>().Deselect();
        selectedTable.Remove(id);
        UnitCount -= 1;


        FormUnitIndexTable(); // добавили порядковые номера юнитов
        UIManager.UpdateUnitIcons(UnitCount); // перерисовали иконки выделенных юнитов
    }


    /// <summary>
    /// Развыделить все юниты
    /// </summary>
    public void deselectAll()
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                //Destroy(selectedTable[pair.Key].GetComponent<selection_component>());

                selectedTable[pair.Key].GetComponent<PlayerStateManager>().Deselect();

                UnitCount = 0;
                //UIManager.UpdateUnitIcons(UnitCount);
            }
        }
        selectedTable.Clear();

        FormUnitIndexTable(); // добавили порядковые номера юнитов
        UIManager.UpdateUnitIcons(UnitCount); // перерисовали иконки выделенных юнитов
    }

    public void MoveModeAggr()
    {

        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {

                selectedTable[pair.Key].GetComponent<PlayerStateManager>().MoveModeAggr();
            }
        }
    }

    public void MoveModePass()
    {
        
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                
                selectedTable[pair.Key].GetComponent<PlayerStateManager>().MoveModePass();
            }
        }
    }

    public void StandGroundOn()
    {

        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                selectedTable[pair.Key].GetComponent<PlayerStateManager>().StandGroundOn();
            }
        }
    }

    public void StandGroundOff()
    {

        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                selectedTable[pair.Key].GetComponent<PlayerStateManager>().StandGroundOff();
            }
        }
    }

    public void FormationRemember()
    {


        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
               selectedTable[pair.Key].GetComponent<PlayerStateManager>().FindCommander();

               selectedTable[pair.Key].GetComponent<PlayerStateManager>().StartFormationRelative();
            }
        }
    }

    public void FormationForget()
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                selectedTable[pair.Key].GetComponent<PlayerStateManager>().FindCommander();

                selectedTable[pair.Key].GetComponent<PlayerStateManager>().TerminateFormation();
            }
        }
    }

    public void UnitsBoardShip(ShipController ship)
    {
        
        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null && !selectedTable[pair.Key].GetComponent<PlayerStateManager>().isDead)
            {
                
                selectedTable[pair.Key].GetComponent<PlayerStateManager>().GetIntoShip(ship);
                
            }
        }
    }

    /// <summary>
    /// Взять экземпляр юнита и передать ему точку назначения навигации
    /// </summary>
    /// <param name="DestinationSet"></param>
    public void MoveUnitsTo(Vector3 DestinationSet)
    {

        //Debug.Log($"move x" + DestinationSet.x);
        //Debug.Log($"move y" + DestinationSet.y);

        foreach (KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                GameObject go = (GameObject)pair.Value;


                if (go.GetComponent<PlayerStateManager>().CurrentState != go.GetComponent<PlayerStateManager>().CargoState)
                {
                    if (go.GetComponent<PlayerStateManager>().CanBeCommanded())
                    {
                        go.GetComponent<PlayerStateManager>().MoveOrders(DestinationSet);
                        //go.GetComponent<PlayerStateManager>().hasMoveOrders = true; 
                        //go.GetComponent<PlayerStateManager>().moveDestination = DestinationSet;
                    }
                }
                else
                {
                    //go.GetComponent<PlayerStateManager>().Unboard(DestinationSet);
                    go.GetComponent<PlayerStateManager>().UnboardLocation = DestinationSet;
                    go.GetComponent<PlayerStateManager>().SwitchState(go.GetComponent<PlayerStateManager>().IdlingState);
                }




            }
        }
    }
}



