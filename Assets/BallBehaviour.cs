using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public float jumpHeight = 2f; // Zýplama yüksekliði
    public float gravity = 2f; // Yerçekimi etkisi
    public float moveSpeed = 5f; // Topun hareket hýzý
    public GameObject[] targets; // Hedef duvarlar
    public float activateThirdWallHeight = 10f; // Üçüncü duvarýn görüneceði yükseklik
    private bool thirdWallActivated = false;
    private Vector2 velocity;
    private Vector2 direction = Vector2.right;

    private Color[] predefinedColors = new Color[]
    {
        Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta,
        Color.black, Color.white, new Color(1f, 0.5f, 0f), new Color(0.5f, 0.25f, 0f) // 10 ana renk
    };

    void Start()
    {
        // Baþlangýçta topun saða doðru hareket etmesini saðla
        velocity = direction * moveSpeed;
        InitializeWallColors(); // Baþlangýçta duvar renklerini ayarla
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol týklama
        {
            // Zýplama hareketini ekle
            velocity.y = jumpHeight;
        }

        // Yerçekimi etkisi uygula
        velocity += Vector2.down * gravity * Time.deltaTime;

        // Sað hareketi uygulama
        transform.Translate(velocity * Time.deltaTime);

        // Topun belirli bir yükseklikte olduðunu kontrol et
        if (!thirdWallActivated && transform.position.y >= activateThirdWallHeight)
        {
            ActivateThirdWall();
            thirdWallActivated = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Çarpýþtýðý nesne bir duvar mý kontrol et
        foreach (GameObject target in targets)
        {
            if (collision.gameObject == target)
            {
                // Renk eþleþmesi kontrolü
                Color ballColor = GetComponent<SpriteRenderer>().color;
                Color targetColor = target.GetComponent<SpriteRenderer>().color;

                if (ballColor == targetColor)
                {
                    // Topun gittiði yönde bulunan duvarýn rengini kontrol et
                    GameObject nextWall = GetNextWallInDirection();
                    if (nextWall != null)
                    {
                        SpriteRenderer nextWallRenderer = nextWall.GetComponent<SpriteRenderer>();
                        if (nextWallRenderer.color == ballColor)
                        {
                            // Renk eþleþirse hem topun hem de duvarýn rengini deðiþtir
                            ChangeColors();

                            // Duvar çarpýþmasýnda yönü deðiþtir
                            direction *= -1;
                            velocity = direction * moveSpeed;
                        }
                        else
                        {
                            // Renkler eþleþmezse topu yok et
                            Destroy(gameObject);
                        }
                    }
                }
                else
                {
                    // Renkler eþleþmezse topu yok et
                    Destroy(gameObject);
                }
            }
        }
    }

    void ActivateThirdWall()
    {
        targets[2].SetActive(true); // Üçüncü duvarý etkinleþtir
    }

    void InitializeWallColors()
    {
        foreach (GameObject target in targets)
        {
            SpriteRenderer targetRenderer = target.GetComponent<SpriteRenderer>();
            Color color1 = GetRandomColor();
            Color color2 = GetRandomColor();

            // Her duvarda 2 renk olmalý
            while (color1 == color2)
            {
                color2 = GetRandomColor(); // Ýki renk aynýysa yeni bir renk seç
            }

            // Duvara iki renk atama (burada, birinci ve ikinci renklerin duvar üzerinde yerleþimini yapabilirsin)
            targetRenderer.color = color1;
        }
    }

    void ChangeColors()
    {
        // Topun rengini rastgele seçilen 10 ana renk arasýndan deðiþtir
        SpriteRenderer ballRenderer = GetComponent<SpriteRenderer>();
        ballRenderer.color = GetRandomColor();

        // Her hedef duvarýn rengini rastgele seçilen 10 ana renk arasýndan deðiþtir
        foreach (GameObject target in targets)
        {
            SpriteRenderer targetRenderer = target.GetComponent<SpriteRenderer>();
            targetRenderer.color = GetRandomColor();
        }
    }

    Color GetRandomColor()
    {
        // Renk listesinden rastgele bir renk döndür
        return predefinedColors[Random.Range(0, predefinedColors.Length)];
    }

    GameObject GetNextWallInDirection()
    {
        // Topun hareket ettiði yönde olan duvarý belirle
        foreach (GameObject target in targets)
        {
            Vector2 toTarget = target.transform.position - transform.position;
            if (Vector2.Dot(toTarget.normalized, direction) > 0.5f && toTarget.magnitude < 1.0f)
            {
                return target;
            }
        }
        return null;
    }
}
