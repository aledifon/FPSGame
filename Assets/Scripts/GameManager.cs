using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    // Static var which contains the single instance of the Singleton
    public static GameManager gm;

    // Audio components
    AudioSource audioSource;
    [SerializeField] AudioClip womanHandsclip;

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
    [SerializeField] float loweringArmsDuration = 2f;
    [SerializeField] float aproachingArmsDuration = 2f;

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
    // Private constructor in order to avoid create new instances through 'new'
    private GameManager() { }

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

        PlayArmsAnimation();
    }

    #region Tweens Animations
    public void PlayArmsAnimation()
    {
        // Stop the general music background
        audioSource.Stop();
        // Enable the Woman's Hands GO
        womanArms.gameObject.SetActive(true);
        // Set the new audio clip & start playing it
        audioSource.clip = womanHandsclip;
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
        sequence.AppendInterval(2f);

        sequence.OnComplete(() =>
        {
            //StartCoroutine(nameof(ArmsAproachingAnimation));
            ArmsAproachingAnimation();
        });
    }
    void ArmsAproachingAnimation()
    {
        // Crear la secuencia
        Sequence sequence = DOTween.Sequence();

        // Primer tween (bajar brazos)
        sequence.Append(womanArms.DOLocalPath(armsAproachingPath, aproachingArmsDuration, pathType));

        // Llamar a la acción después de que termine la secuencia
        sequence.OnKill(() =>
        {
            Debug.Log("Animación 2 completada");
        });

        //yield return null;
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
    //IEnumerator AproachingArmsAnimation()
    //{
    //    isCoroutineRunning = true;

    //    float elapsedTime = 0f;

    //    while (elapsedTime < loweringArmsDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        womanArms.position = Vector3.Lerp(armsMiddlePos.position, armsEndPos.position, elapsedTime / aproachingArmsDuration);
    //        yield return null;
    //    }
    //    // Assure all the doors finish on their right position        
    //    womanArms.position = armsEndPos.position;

    //    isCoroutineRunning = false;
    //}
    #endregion

    void SetBlackScene()
    {

    }
}
