// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FadeDisFromCam" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _FadeStart ("Fade Start", Float) = 0
      _FadeDis ("Fade End", Float) = 10
    }

    SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      LOD 200

      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha

      CGPROGRAM

      #pragma surface surf Lambert finalcolor:mycolor vertex:myvert
      struct Input {
          float2 uv_MainTex;
          half alpha;
      };

      sampler2D _MainTex;
      half _FadeStart;
      half _FadeDis;

      void myvert (inout appdata_full v, out Input data)
      {
          float4 vertexPos = mul (unity_ObjectToWorld, v.vertex);
          float4 camPos = float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0) ;
          float vDis = distance(camPos, vertexPos);
          data.alpha = 1 -  ((vDis-_FadeStart)/(_FadeDis-_FadeStart));
      }

      void mycolor (Input IN, SurfaceOutput o, inout fixed4 color) {
          color.a = IN.alpha;
      }

      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
      }

      ENDCG
    }
    Fallback "Diffuse"
  }
