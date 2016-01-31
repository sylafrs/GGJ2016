using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    //------------------------------------------------------------
    // Variables
    //------------------------------------------------------------
    private string bannerName = "Banner_player_";

    private string medalSubFolder = "Art";
    private string bronzeMedalName = "END_MEDAL_Bronze";
    private string silverMedalName = "END_MEDAL_Silver";
    private string goldMedalName = "END_MEDAL_Gold";

    private Sprite bronzeMedal;
    private Sprite silverMedal;
    private Sprite goldMedal;
    //------------------------------------------------------------

    void Start ()
    {
        //------------------------------------------------------------
        // Resources Loading
        //------------------------------------------------------------
        bronzeMedal = Resources.Load<Sprite>(medalSubFolder + "/" + bronzeMedalName);
        silverMedal = Resources.Load<Sprite>(medalSubFolder + "/" + silverMedalName);
        goldMedal = Resources.Load<Sprite>(medalSubFolder + "/" + goldMedalName);

        //------------------------------------------------------------
        // Set Up of the UI
        //------------------------------------------------------------
        for (int i = 0; i < Game.Instance.Players.Length; i++)
        {
            Player player = Game.Instance.Players[i];
            string instanceBannerName = bannerName + i;
            GameObject banner = GameObject.Find(instanceBannerName);

            InstantiateMedal(player.nbrRoundWin, banner);

            if(player.nbrRoundWin == 3)
            {
                Debug.Log("Player in color : " + player.color.ToString() + " has won the Game ! #YOLOSWAAG");
                banner.GetComponent<Image>().enabled = true;
                banner.transform.FindChild("Crown").GetComponent<Image>().enabled = true;
            }
        }
        //------------------------------------------------------------
    }

    //Instantiate The medal GameObjects
    void InstantiateMedal(int nbMedal, GameObject banner)
    {
		#warning TODO : Use arrays ;-)

        switch (nbMedal)
        {
            case 0:
                break;
            case 1:
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().sprite = bronzeMedal;
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().enabled = true;
                break;
            case 2:
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().sprite = bronzeMedal;
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().enabled = true;

                banner.transform.FindChild("silverMedalPos").GetComponent<Image>().sprite = silverMedal;
                banner.transform.FindChild("silverMedalPos").GetComponent<Image>().enabled = true;
                break;
            case 3:
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().sprite = bronzeMedal;
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().enabled = true;

                banner.transform.FindChild("silverMedalPos").GetComponent<Image>().sprite = silverMedal;
                banner.transform.FindChild("silverMedalPos").GetComponent<Image>().enabled = true;

                banner.transform.FindChild("goldMedalPos").GetComponent<Image>().sprite = goldMedal;
                banner.transform.FindChild("goldMedalPos").GetComponent<Image>().enabled = true;
                break;
            default:
                break;
        }
    }
}
