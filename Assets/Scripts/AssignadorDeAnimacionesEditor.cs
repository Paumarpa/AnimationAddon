using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AssignadorDeAnimaciones))]
public class AssignadorDeAnimacionesEditor : Editor
{


    private int indiceCategoriaSeleccionada;
    private int indiceAnimSeleccionada;
    private bool pause = false;
    private int previousSelectedAnimationIndex = -1;
    private int previousSelectedCategoryIndex = -1;

    private Dictionary<int, Vector3> initialPositions = new Dictionary<int, Vector3>();
    private Dictionary<int, Quaternion> initialRotations = new Dictionary<int, Quaternion>();



    public override void OnInspectorGUI()
    {


        //Estilo para botones
        GUIStyle estiloBoton = new GUIStyle(GUI.skin.button);
        estiloBoton.fontSize = 20;
        

        //inspector
        DrawDefaultInspector();

        //referencia a asignadordeanimaciones
        AssignadorDeAnimaciones assignadorDeAnimaciones = (AssignadorDeAnimaciones)target;

        if (assignadorDeAnimaciones.AnimatorObjetivo != null &&
        assignadorDeAnimaciones.AnimatorObjetivo.avatar != null &&
        !assignadorDeAnimaciones.AnimatorObjetivo.avatar.isHuman)
        {
            EditorGUILayout.HelpBox("El personaje no es humanoide!", MessageType.Warning);
        }



        //Almacenar posicion y rotacion inicial de todos los animators
        int instanceID = assignadorDeAnimaciones.AnimatorObjetivo.GetInstanceID();
        if (!initialPositions.ContainsKey(instanceID))
        {
            initialPositions[instanceID] = assignadorDeAnimaciones.AnimatorObjetivo.transform.position;
            initialRotations[instanceID] = assignadorDeAnimaciones.AnimatorObjetivo.transform.rotation;
        }


        //lista desplegable de seleccionador de categorias
        string[] categorias = assignadorDeAnimaciones.obtenerCategorias();
        int nuevoIndiceCategoriaSeleccionada = EditorGUILayout.Popup("Selecciona la categoria",indiceCategoriaSeleccionada,categorias);


        if (nuevoIndiceCategoriaSeleccionada != indiceCategoriaSeleccionada)
        {
            indiceCategoriaSeleccionada = nuevoIndiceCategoriaSeleccionada;
            indiceAnimSeleccionada = 0;

            
        }
        //animaciones de categoria seleccionada
        assignadorDeAnimaciones.animacionesDisponibles = assignadorDeAnimaciones.GetAnimationsByCategory(categorias[indiceCategoriaSeleccionada]);

        //seleccion animacion
        string[] nombesAnim = assignadorDeAnimaciones.animacionesDisponibles.ConvertAll(a => a.name).ToArray();
        indiceAnimSeleccionada = EditorGUILayout.Popup("Animacion", indiceAnimSeleccionada, nombesAnim);

        //asignar animacion seleccionada al animator
        if((previousSelectedAnimationIndex != indiceAnimSeleccionada || previousSelectedCategoryIndex!= indiceCategoriaSeleccionada)&& assignadorDeAnimaciones.animacionesDisponibles != null && assignadorDeAnimaciones.animacionesDisponibles.Count > 0)
        {
            //assignadorDeAnimaciones.AsignarAnimacion(assignadorDeAnimaciones.animacionesDisponibles[indiceAnimSeleccionada]);

            assignadorDeAnimaciones.AnimatorObjetivo.runtimeAnimatorController = CrearAnimator(assignadorDeAnimaciones.animacionesDisponibles[indiceAnimSeleccionada]);
            previousSelectedAnimationIndex = indiceAnimSeleccionada;
            previousSelectedCategoryIndex = indiceCategoriaSeleccionada;

            assignadorDeAnimaciones.SetInitialPositionAndRotation(initialPositions[instanceID], initialRotations[instanceID]);
            assignadorDeAnimaciones.ResetPositionAndRotation();
        }

        GUILayout.Space(15);


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Anterior"))
        {
            indiceAnimSeleccionada = Mathf.Max(0, indiceAnimSeleccionada - 1);
            //assignadorDeAnimaciones.AsignarAnimacion(assignadorDeAnimaciones.animacionesDisponibles[indiceAnimSeleccionada]);

            assignadorDeAnimaciones.AnimatorObjetivo.runtimeAnimatorController = CrearAnimator(assignadorDeAnimaciones.animacionesDisponibles[indiceAnimSeleccionada]);
            assignadorDeAnimaciones.SetInitialPositionAndRotation(initialPositions[instanceID], initialRotations[instanceID]);
            assignadorDeAnimaciones.ResetPositionAndRotation();
        }
        if (GUILayout.Button("Siguiente"))
        {
            indiceAnimSeleccionada = Mathf.Min(assignadorDeAnimaciones.animacionesDisponibles.Count - 1, indiceAnimSeleccionada + 1);
            //assignadorDeAnimaciones.AsignarAnimacion(assignadorDeAnimaciones.animacionesDisponibles[indiceAnimSeleccionada]);

            assignadorDeAnimaciones.AnimatorObjetivo.runtimeAnimatorController = CrearAnimator(assignadorDeAnimaciones.animacionesDisponibles[indiceAnimSeleccionada]);
            assignadorDeAnimaciones.SetInitialPositionAndRotation(initialPositions[instanceID], initialRotations[instanceID]);
            assignadorDeAnimaciones.ResetPositionAndRotation();
        }
        EditorGUILayout.EndHorizontal();
        //boton Pausar/Reanudar
        string buttonText = pause ? "Reanudar Anim." : "Pausar Anim.";

        if (GUILayout.Button(buttonText, estiloBoton))
        {
            if (pause)
            {
                assignadorDeAnimaciones.ResumeAnim();
                
            }
            else
            {
                assignadorDeAnimaciones.PauseAnim();
                
            }  
        }

        pause = assignadorDeAnimaciones.AnimatorObjetivo.speed == 0;

GUILayout.Space(15);

        float newVel = EditorGUILayout.Slider("Velocidad animacion", assignadorDeAnimaciones.AnimatorObjetivo.speed, 0, 2.5f);
        assignadorDeAnimaciones.SetAnimationSpeed(newVel);


GUILayout.Space(15);


        string RootMotiionTexto = assignadorDeAnimaciones.AnimatorObjetivo.applyRootMotion ? "Desactiva Root Motion" : "Activa Root Motion";


        if (GUILayout.Button(RootMotiionTexto))
        {
            assignadorDeAnimaciones.ToggleRootMotion();
        }


GUILayout.Space(5);

        if (GUILayout.Button("Volver a posicion inicial",estiloBoton))
        {
            assignadorDeAnimaciones.ResetPositionAndRotation();
        }

    }


    // Create a 2D texture
    /*private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }*/

    //crea un animator para cada clip
    
    private RuntimeAnimatorController CrearAnimator(AnimationClip clip)
    {
        // Crear un Animator Controller en tiempo de ejecucion con una sola animacion
        AnimatorController controller = new AnimatorController();
        controller.AddLayer("Default");

        AnimatorStateMachine sm = controller.layers[0].stateMachine;
        AnimatorState state = sm.AddState(clip.name);
        state.motion = clip;

        return controller;
    }
    
}
