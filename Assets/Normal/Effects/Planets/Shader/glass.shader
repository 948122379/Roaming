Shader "glass"
{
//-------------------------------�����ԡ�-----------------------------------------
Properties
{
_Color ("Main Color", Color) = (1,1,1,1)
_MainTex ("Base (RGB) Transparency (A)", 2D) = "white" {}
_Reflections ("Base (RGB) Gloss (A)", Cube) = "skybox" { TexGen CubeReflect }
}
 
//--------------------------------������ɫ����----------------------------------
SubShader
{
//-----------����ɫ����ǩ----------
Tags { "Queue" = "Transparent" }
 
//----------------ͨ��1--------------
Pass
{
Blend SrcAlpha OneMinusSrcAlpha
 
Material
{
Diffuse [_Color]
}
 
Lighting On
SetTexture [_MainTex] {
combine texture * primary double, texture * primary
}
}
 
//----------------ͨ��2--------------
Pass
{
//����������
Blend One One
 
//���ò���
Material
{
Diffuse [_Color]
}
 
//������
Lighting On
 
//���������
SetTexture [_Reflections]
{
combine texture
Matrix [_Reflection]
}
}
}
}