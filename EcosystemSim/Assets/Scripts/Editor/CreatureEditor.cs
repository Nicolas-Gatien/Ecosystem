using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Creature))]
class CreatureEditor : Editor
{
    // TRAITS
    SerializedProperty maxEnergy;
    SerializedProperty regenerationRate;
    SerializedProperty startRecoveryTime;
    SerializedProperty timeBtwDamage;
    SerializedProperty maxHealth;
    SerializedProperty baseSpeed;
    SerializedProperty turnSpeed;
    SerializedProperty size;
    SerializedProperty boneDensity;
    SerializedProperty maturityAge;
    SerializedProperty color;
    SerializedProperty viewRadius;

    // CALCULATED
    SerializedProperty trueSpeed;
    SerializedProperty mass;
    SerializedProperty canBreed;

    // TIME SENSITIVE
    SerializedProperty energy;
    SerializedProperty recoveryTime;
    SerializedProperty timeBeforeNextDamage;
    SerializedProperty age;
    SerializedProperty health;

    void OnEnable()
    {
        maxEnergy = serializedObject.FindProperty("maxEnergy");
        regenerationRate = serializedObject.FindProperty("regenerationRate");
        startRecoveryTime = serializedObject.FindProperty("startRecoveryTime");
        timeBtwDamage = serializedObject.FindProperty("timeBtwDamage");
        maxHealth = serializedObject.FindProperty("maxHealth");
        baseSpeed = serializedObject.FindProperty("baseSpeed");
        turnSpeed = serializedObject.FindProperty("turnSpeed");
        size = serializedObject.FindProperty("size");
        boneDensity = serializedObject.FindProperty("boneDensity");
        maturityAge = serializedObject.FindProperty("maturityAge");
        color = serializedObject.FindProperty("color");
        viewRadius = serializedObject.FindProperty("viewRadius");
        trueSpeed = serializedObject.FindProperty("trueSpeed");
        mass = serializedObject.FindProperty("mass");
        canBreed = serializedObject.FindProperty("canBreed");
        energy = serializedObject.FindProperty("energy");
        recoveryTime = serializedObject.FindProperty("recoveryTime");
        timeBeforeNextDamage = serializedObject.FindProperty("timeBeforeNextDamage");
        age = serializedObject.FindProperty("age");
        health = serializedObject.FindProperty("health");
    }
}

