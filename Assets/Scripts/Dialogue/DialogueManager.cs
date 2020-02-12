using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance = null;

    public GameObject dialoguePanel;
    public Image standingImageL;
    public Image standingImageR;
    public Text nameText;
    public Text dialogueText;
    public Animator panelAnimator;
    public Animator standingAnimator1;
    public Animator standingAnimator2;

    [System.NonSerialized]
    public bool isTalking = false;

    private const string AXIS_TALK = "Fire";
    private const float TYPING_DELAY = 0.05f;
    private bool m_hasTalked = false;

    private AudioSource m_audioSource;
    private bool m_isTriggered = false;
    private bool m_isTyping = false;
    private bool m_isStart = false;

    private Dialogue m_dialogue = null;
    private string m_sentence = null;
    private int m_index = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.instance.isPaused) return;
        if (!isTalking) return;

        float talk = Input.GetAxis(AXIS_TALK);
        if (!m_hasTalked && talk > 0.1f)
        {
            m_hasTalked = true;

            if (isTalking && !m_isTriggered)
            {
                m_isTriggered = true;
                return;
            }

            if (m_isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = m_sentence;
                m_isTyping = false;
            }
            else
            {
                DisplayNextSentence();
            }

            m_audioSource.Play();
        }
        else if (talk == 0f)
        {
            m_hasTalked = false;
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isTalking = true;
        GameManager.instance.isDialogueOn = true;
        GameManager.instance.isPlayerSpelling = false;
        GameManager.instance.RecoverCube(false);

        standingImageL.sprite = dialogue.imagesL[0];
        standingImageR.sprite = dialogue.imagesR[0];
        dialoguePanel.SetActive(true);
        panelAnimator.gameObject.SetActive(true);
        standingAnimator1.gameObject.SetActive(true);
        standingAnimator2.gameObject.SetActive(true);
        panelAnimator.SetBool("IsOpen", true);
        standingAnimator1.SetBool("IsIn", true);
        standingAnimator2.SetBool("IsIn", true);

        m_dialogue = dialogue;
        m_index = 0;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (m_index == m_dialogue.sentences.Length)
        {
            StartCoroutine(EndDialogue());
            return;
        }

        m_sentence = m_dialogue.sentences[m_index];
                
        if (m_dialogue.isSpeakerLeft[m_index])
        {
            nameText.text = m_dialogue.nameL;
            standingAnimator1.SetBool("IsOn", true);
            standingAnimator2.SetBool("IsOn", false);
            standingImageL.sprite = m_dialogue.imagesL[m_dialogue.spriteNs[m_index]];
        }
        else
        {
            nameText.text = m_dialogue.nameR;
            standingAnimator1.SetBool("IsOn", false);
            standingAnimator2.SetBool("IsOn", true);
            standingImageR.sprite = m_dialogue.imagesR[m_dialogue.spriteNs[m_index]];
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(m_sentence));
        m_index++;
    }

    IEnumerator TypeSentence(string sentence)
    {
        m_isTyping = true;
        dialogueText.text = "";

        if (!m_isStart)
        {
            yield return new WaitForSeconds(1f);
            m_isStart = true;
        }

        foreach (char letter in sentence.ToCharArray())
        {
            while (GameManager.instance.isPaused)
            {
                yield return new WaitForSeconds(TYPING_DELAY);
            }

            dialogueText.text += letter;

            yield return new WaitForSeconds(TYPING_DELAY);
        }

        m_isTyping = false;
    }

    public IEnumerator EndDialogue()
    {
        panelAnimator.SetBool("IsOpen", false);
        standingAnimator1.SetBool("IsIn", false);
        standingAnimator2.SetBool("IsIn", false);

        yield return new WaitForSeconds(1f);

        m_index = 0;
        GameManager.instance.isDialogueOn = false;
        GameManager.instance.RecoverCube();
        isTalking = false;
        m_isTriggered = false;

        yield return new WaitForSeconds(1f);

        panelAnimator.gameObject.SetActive(false);
        standingAnimator1.gameObject.SetActive(false);
        standingAnimator2.gameObject.SetActive(false);
    }
}
