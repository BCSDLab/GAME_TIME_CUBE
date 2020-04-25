using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour
{
    private const string AXIS_VERTICAL = "Vertical";
    private const string AXIS_FIRE = "Fire";
    private const float AXIS_MIN = 0.3f;
    private const int BUTTON_N = 3;

    private AudioSource m_audioSource;

    [SerializeField]
    private GameObject cursor = null;
    [SerializeField]
    private Transform buttons = null;
    private int cursorIdx;
    private bool hasMovedCursor;

    void Awake()
    {
        cursorIdx = 0;
        hasMovedCursor = false;
    }

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        MoveCursor();
        SelectButton();
    }

    void MoveCursor()
    {
        // Input.GetKeyDown() 사용 가능
        float vertical = Input.GetAxis(AXIS_VERTICAL);

        if (Mathf.Abs(vertical) < AXIS_MIN)
        {
            hasMovedCursor = false;
        }
        else if (!hasMovedCursor)
        {
            cursorIdx += (vertical > AXIS_MIN) ? -1 : 1;
            cursorIdx = (cursorIdx < 0) ? BUTTON_N - 1 : (cursorIdx >= BUTTON_N) ? 0 : cursorIdx;
            Vector3 cursorPos = cursor.transform.position;
            cursorPos.y = buttons.GetChild(cursorIdx).position.y;
            cursor.transform.position = cursorPos;

            m_audioSource.Play();
            hasMovedCursor = true;
        }
    }

    void SelectButton()
    {
        float fire = Input.GetAxis(AXIS_FIRE);

        if (fire > AXIS_MIN)
        {
            buttons.GetChild(cursorIdx).GetComponent<Button>().onClick.Invoke();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayBossRush()
    {
        SceneManager.LoadScene(4);
    }
}
