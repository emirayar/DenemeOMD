using UnityEngine;

public class InventoryOpener : MonoBehaviour
{
    public GameObject mainInventory; // Ana envanterinizi tutacak olan oyun nesnesi
    public GameObject toolbar; // Toolbar'ý tutacak olan oyun nesnesi
    public GameObject showMainInventoryButton; // Ana envanteri gösteren butonun oyun nesnesi
    private bool inventoryOpen = false; // Envater açýk mý kapalý mý kontrolü için

    void Update()
    {
        // Eðer Escape tuþuna basýlýrsa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Ana envanteri aç veya kapat
            inventoryOpen = !inventoryOpen;
            mainInventory.SetActive(inventoryOpen);

            // Toolbar'ý ve ana envanteri gösteren butonu kapat
            toolbar.SetActive(!inventoryOpen);
            showMainInventoryButton.SetActive(!inventoryOpen);
        }
    }
}

