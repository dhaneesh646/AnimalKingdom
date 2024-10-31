// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'





Shader "JMRSDK/BarrelDistortion" {
    Properties{
        _MainTex("MainTex", 2D) = "white" {} //Texture
        _distortion("distortion", range(-3, 3)) = 0.101 // Total distortion Coeffient
        _cubicDistortion("cubicDistortion", range(0, 3)) = 0.0981 // The cubicDistortion Coeffient
        _scale("scale", range(0, 3)) = 1.0 //Scale of the Distorted Texture
         screenResolution("screenResolution",Vector) = (1920,1080,0,0)
        ipdOffset("ipdOffset",Vector) = (0,0,0,0)        
        _offset("offset",Vector)=(0,0,0,0)
        screenratio("screenratio",Vector) = (0,0,0,0)
         



   }
        SubShader{
            pass {
            Tags{ "LightMode" = "ForwardBase" }
            Cull off
            Fog{  Mode off }
                CGPROGRAM
#pragma vertex Vertex
#pragma fragment Fragment
#pragma target 3.0
#include "UnityCG.cginc"
          //  float     _Intensity_x;
           // float     _Intensity_y;
          //  float _P_x;
          //  float _P_y;
          float4 _offset;
            float _distortion;
            float _cubicDistortion;
            float _scale;
            fixed4 distortionOffset;
            fixed4 screenResolution;
            fixed4 ipdOffset;
            fixed4 rotationVector;
            float4 screenratio;
            fixed4 distortionScale;



          sampler2D _MainTex;
            fixed4 _MainTex_ST;
            struct Vertex2Fragment {
                float4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                float4 worldpos : TEXCOORD1;





          };





          Vertex2Fragment Vertex(appdata_base v) {
                Vertex2Fragment output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                output.worldpos = output.pos;
                //output.uv.x = (v.uv.x * 0.5f)+_distortionOffset;
                return output;
            }



     
         



          float2 brownConradyDistortion(float2 uv) //based on Brown - Conrady Distortion model
            {                    
               float2 h = uv.xy - float2(0.5, 0.5);
                float r2 = h.x * h.x + h.y * h.y;  // positive values of Distortion give barrel distortion, negative give pincushion
                float f = 1.0 + r2 * (_distortion + _cubicDistortion * sqrt(r2));
               




               return f * _scale * h + 0.5;



           }
            
            fixed4 Fragment(Vertex2Fragment input) :COLOR
            {
                fixed4 c;
                float2 smp = input.worldpos.xy;
                
                float2 disp = smp * float2(distortionScale.x, distortionScale.y) + float2(0.5,0.5)+ float2(-distortionOffset.x, -distortionOffset.y);
                float2 offset = float2(1-screenratio.x,1-screenratio.y);
                 disp = disp*screenratio + offset;
                 disp.x = 1.0f - disp.x;                    
                 disp.y = 1.0f - disp.y;    
                 float2 disp1=brownConradyDistortion(input.uv);
                // float2 disp1=getzkhaoDistortion(disp);
                 if (disp1.x < 0 || disp1.x > 1.0 || disp1.y <0 || disp1.y > 1.0)
                    return fixed4(0., 0., 0., 1.);
                    
                disp1 = disp1+float2(ipdOffset.x,ipdOffset.y);
                
                if (disp1.x < 0 || disp1.x > 1.0 || disp1.y <0 || disp1.y > 1.0)
                   return fixed4(0., 0., 0., 1.);




                c = tex2D(_MainTex, disp1+_offset);
                return c;



           }
            ENDCG
        }//





   }
}