using UnityEngine;

public class InventoryOpener : MonoBehaviour
{
    public GameObject mainInventory; // Ana envanterinizi tutacak olan oyun nesnesi
    public GameObject toolbar; // Toolbar'� tutacak olan oyun nesnesi
    public GameObject showMainInventoryButton; // Ana envanteri g�steren butonun oyun nesnesi
    private bool inventoryOpen = false; // Envater a��k m� kapal� m� kontrol� i�in

    void Update()
    {
        // E�er Escape tu�una bas�l�rsa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Ana envanteri a� veya kapat
            inventoryOpen = !inventoryOpen;
            mainInventory.SetActive(inventoryOpen);

            // Toolbar'� ve ana envanteri g�steren butonu kapat
            toolbar.SetActive(!inventoryOpen);
            showMainInventoryButton.SetActive(!inventoryOpen);
        }
    }
}

