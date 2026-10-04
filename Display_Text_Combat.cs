using UnityEngine;
using UnityEngine.UI;
using PixelBattleText;
using TMPro;

namespace AnimatedBattleText.Examples
{
    public class Display_Text_Combat : MonoBehaviour
    {
        private Vector3 textSpawnPosition;
        private TextAnimation lastUsed;

        public TextAnimation ko;
        public TextAnimation lvlUp;
        public TextAnimation premium;
        public TextAnimation spooky;
        public TextAnimation venom;

        public TextAnimation pyro;
        public TextAnimation shock;
        public TextAnimation freeze;

        public TextAnimation metallic;
        public TextAnimation criticalNumber;
        public TextAnimation criticalText;
        public TextAnimation damage;
        public TextAnimation heal;

        public TMP_InputField input;

        public GameObject pallete_0;
        public GameObject pallete_1;
        private bool displayingPallete_0 = true;
        private Image lastButton;

        void Start()
        {
            lastUsed = lvlUp;
            textSpawnPosition = new Vector3(0.5f, 0.65f, 0);
        }

        public Color[] outlineColors;
        public Image textbox;
        public Image button;
        #region UI DUMMY CONTROLS
        public void SetColor(int col)
        {
            textbox.color = outlineColors[col];
            button.color = outlineColors[col];
        }
        private void SwapColors(Image source, Color color)
        {
            if (lastButton)
                lastButton.color = Color.white;
            source.color = color;
        }

        public void SwapEffectPallete()
        {
            displayingPallete_0 = !displayingPallete_0;
            pallete_0.SetActive(displayingPallete_0);
            pallete_1.SetActive(!displayingPallete_0);
        }

        public void ShowInputText()
        {
            var text = input.text == "" ? "JUST TYPE SOMETHING..." : input.text;
            PixelBattleTextController.DisplayText(text, lastUsed, textSpawnPosition);
        }
        public void DisplayDamage(int daño)
        {
            //Despliega el daño generico hecho
            PixelBattleTextController.DisplayText(daño.ToString(), damage, textSpawnPosition);
            lastUsed = damage;
        }
        public void DisplayVictory()
        {
            //Despliega el texto "VICTORY" con la animación de premium, ideal para cuando ganas una batalla
            PixelBattleTextController.DisplayText("VICTORY", premium,textSpawnPosition);
            lastUsed = premium;
        }
        public void DisplayHeal(int Curar)
        {
            //Despliega la cantidad de curación que se ha hecho
            PixelBattleTextController.DisplayText(Curar.ToString(), heal, textSpawnPosition);
            lastUsed = heal;
        }
        public void DisplayAddArmor(int Armor)
        {
            //Despliega el numero del blindaje añadido, ideal para personajes con armadura
            PixelBattleTextController.DisplayText(Armor.ToString(),metallic,textSpawnPosition);
            lastUsed = metallic;
        }
        public void DisplayDamageArmor(int Armor)
        {
            //Despliega el daño al blindaje, ideal para personajes con armadura
            PixelBattleTextController.DisplayText(Armor.ToString(), ko, textSpawnPosition);
            lastUsed = ko;
        }
        public void DisplayState(string State)
        {
            //Despliega el Nombre del estado alterado aplicado; Como veneno, quemadura, etc.
            PixelBattleTextController.DisplayText(State, spooky,textSpawnPosition);
            lastUsed = spooky;
        }
        public void DisplayKO()
        {
            //Despliega el texto "KO" con la animación de KO, ideal para cuando eliminas un Sub Jefe
            PixelBattleTextController.DisplayText("KO", ko, textSpawnPosition);
            lastUsed = ko;
        }
        public void DisplayLvlUp()
        {
            //Despliega el texto "LEVEL UP!" con la animación de lvlUp, ideal para cuando subes de nivel
            PixelBattleTextController.DisplayText("LEVEL UP!",lvlUp,textSpawnPosition);
            lastUsed = lvlUp;
        }
        public void DisplayPyro(int Pyro)
        {
            //Despliega el daño de fuego, ideal para magia de fuego de Frogzard
            PixelBattleTextController.DisplayText(Pyro.ToString(),pyro,textSpawnPosition);
            lastUsed = pyro;
        }
        public void DisplayFreeze(int Freeze)
        {
            //Despliega el daño de Agua/Hielo, ideal para magia de agua/hielo de Frogzard
            PixelBattleTextController.DisplayText(Freeze.ToString(),freeze,textSpawnPosition);
            lastUsed = freeze;
        }

        public void DisplayShock(int Electric)
        {
            //Despliega el daño eléctrico, ideal para magia electrica de Frogzard
            PixelBattleTextController.DisplayText(Electric.ToString(),shock,textSpawnPosition);
            lastUsed = shock;
        }
        public void DisplayVenom(int veneno)
        {
            //Despliega el daño de veneno, ideal para cuando una unidad esta envenenada y recibe daño por eso
            PixelBattleTextController.DisplayText(veneno.ToString(),venom,textSpawnPosition);
            lastUsed = venom;
        }
        public void DisplayCrit(int critico)
        {
            //Despliega el numero del daño crítico
            PixelBattleTextController.DisplayText(critico.ToString(),criticalNumber,textSpawnPosition);
            //Despliega el texto "CRITICAL!" un poco más arriba del número
            PixelBattleTextController.DisplayText("CRITICAL!",criticalText,textSpawnPosition + new Vector3(0, 0.15f, 0));
            lastUsed = criticalNumber;
        }

        #endregion
    }
}