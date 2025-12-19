using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro için þart
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ScoreboardManager : MonoBehaviour
{
    // --- BU KUTUCUKLAR INSPECTOR'DA GÖRÜNMELÝ ---
    [Header("UI Baðlantýlarý")]
    public GameObject satirPrefab;   // Buraya mavi Text (TMP) dosyasýný atacaksýn
    public Transform listeContent;   // Buraya Content objesini atacaksýn
    public TextMeshProUGUI durumYazisi; // Buraya "Yükleniyor..." yazýsýný at (Varsa)

    FirebaseFirestore db;
    FirebaseAuth auth;

    void Start()
    {
        // Veritabanýný baþlat
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // Verileri çekmeye baþla
        VerileriGetir();
    }

    void VerileriGetir()
    {
        if (auth.CurrentUser == null)
        {
            if (durumYazisi) durumYazisi.text = "Giriþ yapýlmamýþ!";
            return;
        }

        string userId = auth.CurrentUser.UserId;
        if (durumYazisi) durumYazisi.text = "Veriler yükleniyor...";

        // Firestore'dan veriyi çek
        db.Collection("Users").Document(userId).Collection("Gecmis")
            .OrderByDescending("tarih") // En yeni en üstte olsun
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    if (durumYazisi) durumYazisi.text = "Veri alýnamadý!";
                    return;
                }

                // Eski listeyi temizle (Öncekileri sil)
                foreach (Transform child in listeContent)
                {
                    Destroy(child.gameObject);
                }

                QuerySnapshot snapshot = task.Result;

                if (snapshot.Count == 0)
                {
                    if (durumYazisi) durumYazisi.text = "Henüz kayýt yok.";
                    return;
                }

                if (durumYazisi) durumYazisi.text = ""; // Hata yoksa yazýyý temizle

                // Gelen her kayýt için bir satýr oluþtur
                foreach (DocumentSnapshot belge in snapshot.Documents)
                {
                    Dictionary<string, object> veri = belge.ToDictionary();

                    // Verinin içinde 'hedef' ve 'puan' var mý kontrol et
                    if (veri.ContainsKey("hedef") && veri.ContainsKey("puan"))
                    {
                        string kelime = veri["hedef"].ToString();
                        string puan = veri["puan"].ToString();

                        // PREFAB'I ÇOÐALT (Satýr üret)
                        GameObject yeniSatir = Instantiate(satirPrefab, listeContent);

                        // Yazýsýný güncelle
                        yeniSatir.GetComponent<TextMeshProUGUI>().text = $"{kelime} : {puan} Puan";
                    }
                }
            });
    }

    // Ana Menüye Dönüþ Butonu Ýçin
    public void AnaMenuyeDon()
    {
        SceneManager.LoadScene("MainMenu"); // Senin sahne adýn 'MainMenu' ise
    }
}