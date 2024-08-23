using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public float jumpHeight = 2f; // Z�plama y�ksekli�i
    public float gravity = 2f; // Yer�ekimi etkisi
    public float moveSpeed = 5f; // Topun hareket h�z�
    public GameObject[] targets; // Hedef duvarlar
    public float activateThirdWallHeight = 10f; // ���nc� duvar�n g�r�nece�i y�kseklik
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
        // Ba�lang��ta topun sa�a do�ru hareket etmesini sa�la
        velocity = direction * moveSpeed;
        InitializeWallColors(); // Ba�lang��ta duvar renklerini ayarla
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol t�klama
        {
            // Z�plama hareketini ekle
            velocity.y = jumpHeight;
        }

        // Yer�ekimi etkisi uygula
        velocity += Vector2.down * gravity * Time.deltaTime;

        // Sa� hareketi uygulama
        transform.Translate(velocity * Time.deltaTime);

        // Topun belirli bir y�kseklikte oldu�unu kontrol et
        if (!thirdWallActivated && transform.position.y >= activateThirdWallHeight)
        {
            ActivateThirdWall();
            thirdWallActivated = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // �arp��t��� nesne bir duvar m� kontrol et
        foreach (GameObject target in targets)
        {
            if (collision.gameObject == target)
            {
                // Renk e�le�mesi kontrol�
                Color ballColor = GetComponent<SpriteRenderer>().color;
                Color targetColor = target.GetComponent<SpriteRenderer>().color;

                if (ballColor == targetColor)
                {
                    // Topun gitti�i y�nde bulunan duvar�n rengini kontrol et
                    GameObject nextWall = GetNextWallInDirection();
                    if (nextWall != null)
                    {
                        SpriteRenderer nextWallRenderer = nextWall.GetComponent<SpriteRenderer>();
                        if (nextWallRenderer.color == ballColor)
                        {
                            // Renk e�le�irse hem topun hem de duvar�n rengini de�i�tir
                            ChangeColors();

                            // Duvar �arp��mas�nda y�n� de�i�tir
                            direction *= -1;
                            velocity = direction * moveSpeed;
                        }
                        else
                        {
                            // Renkler e�le�mezse topu yok et
                            Destroy(gameObject);
                        }
                    }
                }
                else
                {
                    // Renkler e�le�mezse topu yok et
                    Destroy(gameObject);
                }
            }
        }
    }

    void ActivateThirdWall()
    {
        targets[2].SetActive(true); // ���nc� duvar� etkinle�tir
    }

    void InitializeWallColors()
    {
        foreach (GameObject target in targets)
        {
            SpriteRenderer targetRenderer = target.GetComponent<SpriteRenderer>();
            Color color1 = GetRandomColor();
            Color color2 = GetRandomColor();

            // Her duvarda 2 renk olmal�
            while (color1 == color2)
            {
                color2 = GetRandomColor(); // �ki renk ayn�ysa yeni bir renk se�
            }

            // Duvara iki renk atama (burada, birinci ve ikinci renklerin duvar �zerinde yerle�imini yapabilirsin)
            targetRenderer.color = color1;
        }
    }

    void ChangeColors()
    {
        // Topun rengini rastgele se�ilen 10 ana renk aras�ndan de�i�tir
        SpriteRenderer ballRenderer = GetComponent<SpriteRenderer>();
        ballRenderer.color = GetRandomColor();

        // Her hedef duvar�n rengini rastgele se�ilen 10 ana renk aras�ndan de�i�tir
        foreach (GameObject target in targets)
        {
            SpriteRenderer targetRenderer = target.GetComponent<SpriteRenderer>();
            targetRenderer.color = GetRandomColor();
        }
    }

    Color GetRandomColor()
    {
        // Renk listesinden rastgele bir renk d�nd�r
        return predefinedColors[Random.Range(0, predefinedColors.Length)];
    }

    GameObject GetNextWallInDirection()
    {
        // Topun hareket etti�i y�nde olan duvar� belirle
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
