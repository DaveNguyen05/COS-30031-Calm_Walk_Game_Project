using UnityEngine;
using UnityEngine.UI;

namespace FloorFixerV2.HotbarFix
{
    public class HotbarSelectionController : MonoBehaviour
    {
        private Image[] slots = new Image[9];

        public Color normalColour = new Color(0.94f, 0.94f, 0.94f, 1f);
        public Color selectedColour = new Color(1f, 0.65f, 0.1f, 1f);

        private int selectedSlot = 0;

        private void Start()
        {
            FindSlots();
            SelectSlot(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
            if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
            if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
            if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(7);
            if (Input.GetKeyDown(KeyCode.Alpha9)) SelectSlot(8);
        }

        private void FindSlots()
        {
            for (int i = 0; i < 9; i++)
            {
                Transform slot = transform.Find("Slot " + (i + 1));

                if (slot != null)
                {
                    slots[i] = slot.GetComponent<Image>();
                }
                else
                {
                    Debug.LogError("Could not find Slot " + (i + 1));
                }
            }
        }

        public void SelectSlot(int index)
        {
            selectedSlot = index;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                    continue;

                if (i == selectedSlot)
                    slots[i].color = selectedColour;
                else
                    slots[i].color = normalColour;
            }

            Debug.Log("Selected Slot " + (selectedSlot + 1));
        }
    }
}