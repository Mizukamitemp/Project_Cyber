using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;
//using static System.Net.Mime.MediaTypeNames;

public class UIManager : MonoBehaviour
{
    selected_dictionary selected_table; // таблица выбранных юнитов
    public GameObject Player; // У него находим СЛОВАРЬ"
    public TextMeshProUGUI ConsoleText;

    public GameObject Father; //ребенок Canvas, в него пихаем новые UI элементы чтобы курсор был всегда сверху




    public GameObject PortraitPrefab;
    public GameObject HealthPrefab;//единичка здоровья которая множится
    public List<GameObject> HealthPrefabs = new List<GameObject>(); // Список квадратиков здоровья
    public List<GameObject> FatiguePrefabs = new List<GameObject>();   // Список квадратиков утомления



    public List<GameObject> UIPortraits1 = new List<GameObject>();

    public GameObject PortraitHealthPrefab;

    // Спрайты для здоровья и усталости
    public Sprite healthSprite;
    public Sprite noHealthSprite;
    public Sprite fatigueSprite;
    public Sprite noFatigueSprite;

    public List<GameObject> UIPortraitsHealth1 = new List<GameObject>();    


    public GameObject PortraitNumPrefab;
    public List<GameObject> UIPortraitsNum1 = new List<GameObject>();


    public Sprite[] UnitNumberIcons;

    public int portraits_count = 0;

    //25 feb 24



    // Start is called before the first frame update
    void Start()
    {
        selected_table = Player.GetComponent<selected_dictionary>();   //У нас есть на игроке такой скрипт


        for (int i = 0; i < 8; i++)
        {

            Vector3 position = new Vector3(1000 + i * 80, -100f, 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);

            UIPortraits1.Add(Object.Instantiate(PortraitPrefab, position, rotation));
            UIPortraits1[i].SetActive(value: false);
            UIPortraits1[i].transform.SetParent(Father.transform, worldPositionStays: false);

            Vector3 position2 = new Vector3(46f, -35f, 0f);
            UIPortraitsHealth1.Add(Object.Instantiate(PortraitHealthPrefab, position2, rotation));
            UIPortraitsHealth1[i].transform.SetParent(UIPortraits1[i].transform, worldPositionStays: false);

            Vector3 position3 = new Vector3(22f, -32f, 0f);
            UIPortraitsNum1.Add(Object.Instantiate(PortraitNumPrefab, position3, rotation));
            UIPortraitsNum1[i].transform.SetParent(UIPortraits1[i].transform, worldPositionStays: false);
        }

        // создаем и прячем 10 квадратиков здоровья
        for (int i = 0; i < 10; i++)
        {

            Vector3 position = new Vector3(22 + i * 22, 60, 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);

            HealthPrefabs.Add(Object.Instantiate(HealthPrefab, position, rotation));
            HealthPrefabs[i].SetActive(true);
            HealthPrefabs[i].transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);

        }

        // создаем и прячем 10 квадратиков усталости
        for (int i = 0; i < 10; i++)
        {

            Vector3 position = new Vector3(22 + i * 22, 30, 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);

            FatiguePrefabs.Add(Object.Instantiate(HealthPrefab, position, rotation));
            FatiguePrefabs[i].SetActive(true);
            FatiguePrefabs[i].transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);

        }

    }

    // Update is called once per frame
    void Update()
    {

        // это мы обновляем здоровье
        int health = Player.GetComponent<PlayerController>().GetHealth();
        int maxhealth = Player.GetComponent<PlayerController>().GetMaxHealth();
        int showhealth = Mathf.RoundToInt(health * 10 / maxhealth );

        for (int i = 0; i < 10; i++)
        {
            if (i < showhealth)
            {
                HealthPrefabs[i].GetComponent<Image>().sprite = healthSprite;
            }
            else HealthPrefabs[i].GetComponent<Image>().sprite = noHealthSprite;
        }




        // это мы обновляем утомление
        int fatigue = Player.GetComponent<PlayerController>().GetFatigue();
        int maxFatigue = Player.GetComponent<PlayerController>().GetMaxFatigue();
        int showfatigue = Mathf.RoundToInt(fatigue * 10 / maxFatigue);

        for (int i = 0; i < 10; i++)
        {
            if (i < showfatigue)
            {
                FatiguePrefabs[i].GetComponent<Image>().sprite = fatigueSprite;
            }
            else FatiguePrefabs[i].GetComponent<Image>().sprite = noFatigueSprite;
        }



    }

    public void UpdateUnitIcons(int NumUnits)
    {


        /*
         * 
         * 
         * 
         * 
 
        for (int i = 0; i < 8; i++)
        {

            if (selected_table.ExistsUnit(i+1))
            {

                if (selected_table.GetPortrait(i + 1) != null)
                {
                    UIPortraits1[i].GetComponent<Image>().sprite = selected_table.GetPortrait(i + 1);
                    UIPortraits1[i].GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
                    UIPortraitsHealth1[i].GetComponent<TextMeshProUGUI>().text = selected_table.GetUnitHealth(i + 1).ToString();
                    UIPortraitsNum1[i].GetComponent<Image>().sprite = UnitNumberIcons[selected_table.GetUnitNumber(i + 1)];
                }
                else Debug.Log($"Sprite is null");

                UIPortraits1[i].SetActive(true);
                portraits_count=i+1;
            }
            else
            {
                UIPortraits1[i].SetActive(false);
            }

        }
         * 
         * 
         * 
         * 
         * 
         */






        for (int i = 0; i < 8; i++)
        {
            if (selected_table.ExistsUnitInArmy(i + 1))
            {
                selected_table.CheckDeadInArmy(i + 1);
                if (selected_table.GetPortraitInArmy(i + 1) != null)
                {
                    Vector3 vector = new Vector3(1000 - selected_table.UnitsExistInArmy() * 80 / 2 + i * 80, -100f, 0f);
                    UIPortraits1[i].GetComponent<RectTransform>().anchoredPosition = vector;
                    UIPortraits1[i].GetComponent<Image>().sprite = selected_table.GetPortraitInArmy(i + 1);
                    UIPortraits1[i].GetComponent<UI_UnitIcon>().UnitIndex = i + 1;
                    if (selected_table.UnitIsSelected(i + 1))
                    {
                        UIPortraits1[i].GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
                    }
                    else
                    {
                        UIPortraits1[i].GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.5f);
                    }
                    UIPortraitsHealth1[i].GetComponent<TextMeshProUGUI>().text = selected_table.GetUnitHealthArmy(i + 1).ToString();
                    UIPortraitsNum1[i].GetComponent<Image>().sprite = UnitNumberIcons[i + 1];
                }
                UIPortraits1[i].SetActive(value: true);
            }
            else
            {
                UIPortraits1[i].SetActive(value: false);
            }
        }


    }

    public void ShowMessage(string msg)
    {
        ConsoleText.text = msg;
        //ConsoleText.text += "\n";
        //ConsoleText.text += msg;
    }
    public void ClearSelection()
    {
        //ConsoleText.text += "\nClear selection";
        ShowMessage("Clear selection");
        selected_table.deselectAll();
    }

    public void MoveModeAggr()
    {
        ShowMessage("Movement mode: Aggressive");
        selected_table.MoveModeAggr();
    }

    public void MoveModePass()
    {
        ShowMessage("Movement mode: Ignore Targets");
        selected_table.MoveModePass();
    }

    public void StandGroundOn()
    {
        ShowMessage("Stand Ground: On");
        selected_table.StandGroundOn();
    }

    public void StandGroundOff()
    {
        ShowMessage("Stand Ground: Off");
        selected_table.StandGroundOff();
    }

    public void FormationRemember()
    {
        ShowMessage("Follow this Formation");
        selected_table.FormationRemember();
    }

    public void FormationForget()
    {
        ShowMessage("Formation disabled");
        selected_table.FormationForget();
    }

    public void SelectOnlyMe(int UnitIndex)
    {
        if (portraits_count == 1)
        {
            selected_table.deselectAll();
        }
        else
        {
            selected_table.SelectOnlyMe(UnitIndex);
        }
    }

    public void UnitIconClicked(int UnitIndex)
    {
        if (selected_table.UnitIsSelected(UnitIndex))
        {
            selected_table.RemoveSelectionFromArmy(UnitIndex);
        }
        else
        {
            selected_table.AddSelectionFromArmy(UnitIndex);
        }
        
    }

}
