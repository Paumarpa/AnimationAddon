using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;



public class AssignadorDeAnimaciones : MonoBehaviour
{
    
    public Animator AnimatorObjetivo//animator del personaje objetivo, solo coge avatares, o personajes humanoides
    {
        get { return _animatorObjetivo; }
        set
        {
            // Para que el animator solo funcione con personajes humanoides
            if (value != null && value.avatar != null && value.avatar.isHuman)
            {
                _animatorObjetivo = value;
            }
        }
    }
    public Animator _animatorObjetivo;

    private string categoria; //Categoria de la animacion seleccionada
    public List<AnimationClip> animacionesDisponibles; //Lista de animaciones disponibles
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private void Awake()
    {
        if (AnimatorObjetivo != null)
        {
            initialPosition = AnimatorObjetivo.transform.position;
            initialRotation = AnimatorObjetivo.transform.rotation;
        }
    }
    //TODO: acoplar la funcionalidad overrideController
    public void AsignarAnimacion(AnimationClip animationClip)
    {
        // Crea un nuevo AnimatorOverrideController basado en el AnimatorController del objetivo
        AnimatorOverrideController overrideController = new AnimatorOverrideController(AnimatorObjetivo.runtimeAnimatorController);

        // Asigna la animacion seleccionada al overrideController
        overrideController["NombreDelEstadoDeAnimacion"] = animationClip;

        // Aplica el overrideController al objetivo
        AnimatorObjetivo.runtimeAnimatorController = overrideController;
    }

    public void CargaAnimacionesPorCategoria()
    {
        // Carga las animaciones de la categoria seleccionada en availableAnimations
        
        animacionesDisponibles = GetAnimationsByCategory(categoria);
    }

#if UNITY_EDITOR

    //coge los clips por categoria de tu carpeta de Animaciones 
    public List<AnimationClip> GetAnimationsByCategory(string categoria)
    {
        //Aqui obtendremos una lista de las animpaciones existentes embase a una categoria

        string animFolderPath = "Assets/Animaciones";//directorio carpeta animaciones
        string categoryFolderPath = Path.Combine(animFolderPath, categoria);

        //obtiene los archivos .anim de la carpeta de categoria seleccionada
        string[] animFiles = Directory.GetFiles(categoryFolderPath, "*.anim", SearchOption.TopDirectoryOnly);

        List<AnimationClip> animaciones = new List<AnimationClip>();
        foreach(string animFile in animFiles)
        {
            //convertimos la ruta absoluta del archivo de animacion en una ruta relativa al archivo Assets del proyecto
            string relativePath = animFile.Replace("\\", "/").Replace(Application.dataPath, "Assets");

            //Cargamos el archivo AnimationClip
            AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(relativePath);

            if(animationClip != null)
            {
                animaciones.Add(animationClip);
            }
        }

        return animaciones;
    }
#endif
    public string[] obtenerCategorias()
    {
        
        // Obtenemos la lista de categorias segun las carpetas del poryecto

        string assetFolderPath = "Assets";//carpeta assets
        string animFolderPath = "Assets/Animaciones";//carpeta animaciones

        //lista de directorios dentro de la carpeta animaciones
        string[] directorios = Directory.GetDirectories(animFolderPath, "*", SearchOption.TopDirectoryOnly);

        string[] categorias = directorios.Select(directory =>
        {
            string relativePath = directory.Replace(assetFolderPath, "").TrimStart('/');
            string nombreCategoria = Path.GetFileName(relativePath);
            return nombreCategoria;
        }).ToArray();

        return categorias;

    }

    public void ResetPositionAndRotation()
    {
        if (AnimatorObjetivo != null)
        {
            AnimatorObjetivo.transform.position = initialPosition;
            AnimatorObjetivo.transform.rotation = initialRotation;
        }
    }

    public void SetInitialPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        initialPosition = position;
        initialRotation = rotation;
    }

    public void PauseAnim()
    {
        if(AnimatorObjetivo != null)
        {
            AnimatorObjetivo.speed = 0;
        }
    }

    public void ResumeAnim()
    {
        if (AnimatorObjetivo != null)
        {
            AnimatorObjetivo.speed = 1;
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        if(AnimatorObjetivo != null)
        {
            AnimatorObjetivo.speed = speed;
        }
    }

    public void ToggleRootMotion()
    {
        if (AnimatorObjetivo != null)
        {
            AnimatorObjetivo.applyRootMotion = !AnimatorObjetivo.applyRootMotion;
        }
    }
}
