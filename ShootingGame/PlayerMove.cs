using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    public float speed = 0.05f;

    Transform tr;

    public Vector2 limitPoint1;
    public Vector2 limitPoint2;

    public GameObject prefabBullet;

    void Start()
    {
        tr = GetComponent<Transform>();

        StartCoroutine(FireBullet());
    }

    IEnumerator FireBullet()
    {
        while (true)
        {
            Instantiate(prefabBullet, tr.position, Quaternion.identity);

            yield return new WaitForSeconds(0.3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        tr = GetComponent<Transform>();

        float x = Input.GetAxis("Horizontal") * speed;
        float y = Input.GetAxis("Vertical") * speed;

        tr.Translate(new Vector2(x,y));
    }
}
