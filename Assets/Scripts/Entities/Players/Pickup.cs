using UnityEngine;

public class Flash : MonoBehaviour
{
    public GameObject FlashLightOnPlayer;
    public GameObject pickuptext;

    void Start()
    {
        FlashLightOnPlayer.SetActive(false);
        pickuptext.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pickuptext.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                gameObject.SetActive(false);
                FlashLightOnPlayer.SetActive(true);
                pickuptext.SetActive(false);
            }
        }
    }
}
