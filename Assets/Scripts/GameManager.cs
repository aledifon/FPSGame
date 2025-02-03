using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    // Static var which contains the single instance of the Singleton
    private static GameManager gm;
    public static GameManager Gm 
    { 
        get 
        { 
            if (gm == null)
            {
                gm = FindAnyObjectByType<GameManager>();

                if (gm == null)
                {
                    GameObject go = new GameObject("GameManager");
                    gm = go.AddComponent<GameManager>();
                }
            }
            return gm; 
        } 
    }

    [SerializeField] private InteractuableObject lastDocumentObject; // Ref. to the Paper2 object

    [SerializeField] private AudioSource clockAudioSource;
    [SerializeField] private AudioSource womanSongAudioSource;    

    private FadeManager fadeManager;

    // Audio components
    AudioSource audioSource;
    [SerializeField] AudioClip womanHandsclip;
    [SerializeField] AudioClip finalHandsclip;

    Transform playerTransform;

    // Woman Hands Positions and Path
    [SerializeField] Transform womanArms;
    [SerializeField] Transform armsStartPos;
    [SerializeField] Transform armsMiddlePos;
    [SerializeField] Transform armsEndPos;
    private Vector3[] armsLoweringPath;
    private Vector3[] armsAproachingPath;
    [SerializeField] private PathType pathType;

    // Woman Arms Duration
    [SerializeField] float loweringArmsDuration;
    [SerializeField] float aproachingArmsDuration;

    bool isCoroutineRunning;

    private Tween t;    

    // Start is called before the first frame update
    void Awake()
    {
        // Saves the current instance of GameManager to the static var.
        if (gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject); // Avoids the GO will be destroyed when the Scene changes
        }
        // If does already exists an instance of the GameManager then we'll destroy
        // this GO
        else
            Destroy(gameObject);
    }    

    private void Start()
    {
        playerTransform = womanArms.parent;

        armsLoweringPath = new[]
        {
            playerTransform.InverseTransformPoint(armsStartPos.position),
            playerTransform.InverseTransformPoint(armsMiddlePos.position)
        };

        armsAproachingPath = new[]
        {
            playerTransform.InverseTransformPoint(armsMiddlePos.position),
            playerTransform.InverseTransformPoint(armsEndPos.position)
        };

        audioSource = GetComponent<AudioSource>();
        fadeManager = GetComponent<FadeManager>();          
                    
        //PlayArmsAnimation();

        // Subscription to the Start & Stop reading events from InteractuableObject
        InteractuableObject.OnReadingFinished += PlayArmsAnimationHandler;
        InteractuableObject.OnReadingStarted += StopGeneralAudio;
    }

    #region Tweens Animations

    void PlayArmsAnimationHandler(InteractuableObject sender)
    {
        // Check if the Object which triggered the event was Paper2
        if (sender == lastDocumentObject) 
            StartCoroutine(nameof(PlayArmsAnimation));
    }
    IEnumerator PlayArmsAnimation()
    {        
        yield return new WaitForSeconds(3);

        audioSource.Play();
        // Finally start the Woman's Hands animation
        ArmsLoweringAnimation();
    }    
    void ArmsFullAnimation()
    {
        // Crear la secuencia
        Sequence sequence = DOTween.Sequence();        

        // Primer tween (bajar brazos)
        sequence.Append(womanArms.DOLocalPath(armsLoweringPath, loweringArmsDuration, pathType));

        // Agregar intervalo (espera de 2 segundos)
        sequence.AppendInterval(2f);

        // Segundo tween (elevar brazos)
        sequence.Append(womanArms.DOLocalPath(armsAproachingPath, aproachingArmsDuration, pathType));

        // Al terminar todo
        sequence.OnKill(() => Debug.Log("La secuencia completa ha terminado"));
    }
    void ArmsLoweringAnimation()
    {
        // Crear la secuencia
        Sequence sequence = DOTween.Sequence();

        // Primer tween (bajar brazos)
        sequence.Append(womanArms.DOLocalPath(armsLoweringPath, loweringArmsDuration, pathType));        

        // Agregar intervalo (espera de 2 segundos)
        //sequence.AppendInterval(1f);

        sequence.OnComplete(() =>
        {
            //StartCoroutine(nameof(ArmsAproachingAnimation));            
            StartCoroutine(nameof(PlayFinalAudioClip));           
        });
    }
    void ArmsAproachingAnimation()
    {
        // Crear la secuencia
        Sequence sequence = DOTween.Sequence();

        // Primer tween (bajar brazos)
        sequence.Append(womanArms.DOLocalPath(armsAproachingPath, aproachingArmsDuration, pathType));

        // Llamar a la acción después de que termine la secuencia
        sequence.OnComplete(() =>
        {
            Debug.Log("Animación 2 completada");
            // Stop the final audio Clip
            audioSource.Stop();
            // Fade to Black
            fadeManager.FadeToBlack();
        });

        //yield return null;

        // Unscription from the event just after finish
        InteractuableObject.OnReadingFinished -= PlayArmsAnimationHandler;
    }
    #endregion

    #region NoTweensAnimations
    //IEnumerator LoweringArmsAnimation()
    //{
    //    isCoroutineRunning = true;

    //    float elapsedTime = 0f;

    //    while (elapsedTime < loweringArmsDuration)
    //    {
    //        elapsedTime += Time.deltaTime;            
    //        womanArms.position = Vector3.Lerp(armsStartPos.position, armsMiddlePos.position, elapsedTime / loweringArmsDuration);
    //        yield return null;
    //    }
    //    // Assure all the doors finish on their right position        
    //    womanArms.position = armsMiddlePos.position;        

    //    isCoroutineRunning = false;        
    //}
    IEnumerator AproachingArmsAnimation()
    {
        isCoroutineRunning = true;

        float elapsedTime = 0f;

        while (elapsedTime < aproachingArmsDuration)
        {
            elapsedTime += Time.deltaTime;
            womanArms.position = Vector3.Lerp(armsMiddlePos.position, armsEndPos.position, elapsedTime / aproachingArmsDuration);
            yield return null;
        }
        // Assure all the doors finish on their right position        
        womanArms.position = armsEndPos.position;

        // Stop the final audio Clip
        audioSource.Stop();
        // Fade to Black
        fadeManager.FadeToBlack();

        isCoroutineRunning = false;
    }
    #endregion

    IEnumerator PlayFinalAudioClip()
    {
        //audioSource.Stop();
        // Set the new audio clip & start playing it        
        audioSource.clip = finalHandsclip;
        audioSource.time = 1f;
        audioSource.Play();
        yield return new WaitForSeconds(0.7f);
        //ArmsAproachingAnimation();
        StartCoroutine(nameof(AproachingArmsAnimation));

    }
    void StopGeneralAudio(InteractuableObject sender)
    {
        // Check if the Object which triggered the event was Paper2
        if (sender == lastDocumentObject)
        {
            // Stop the Woman song audio
            womanSongAudioSource.Stop();
            // Stop the Alarm Clock audio
            clockAudioSource.Stop();
            // Stop the general music background
            audioSource.Stop();
            // Enable the Woman's Hands GO
            womanArms.gameObject.SetActive(true);
            // Set the new audio clip & start playing it
            audioSource.clip = womanHandsclip;
            // Set again the Volume to the maximum            
            audioSource.volume = 1f;
            // Unscription from the event just after finish
            InteractuableObject.OnReadingStarted -= StopGeneralAudio;
        }        
    }

    void SetBlackScene()
    {

    }
}
