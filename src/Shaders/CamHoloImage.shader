Shader "Unlit/FCAPHoloImage"
{
    Properties
    {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    SubShader
    {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Fog { Color(0,0,0,0) }
		Lighting Off
		Cull Off //we can turn backface culling off because we know nothing will be facing backwards

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "_ShaderFix.cginc"

            sampler2D _MainTex;
            sampler2D _LevelTex;
            sampler2D _NoiseTex;
            sampler2D _NoiseTex2;
            sampler2D _CloudsTex;
            sampler2D _PalTex;
            uniform float _fogAmount;
            uniform half4 _AboveCloudsAtmosphereColor;

            #if defined(SHADER_API_PSSL)
            sampler2D _GrabTexture;
            #else
            sampler2D _GrabTexture : register(s0);
            #endif

            uniform float _RAIN;

            uniform float4 _spriteRect;
            uniform float2 _screenSize;

            uniform float _hologramThreshold;
            uniform float _fcapThresh;

            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float2 scrPos : TEXCOORD1;
                float4 clr : COLOR;
            };

            float4 _MainTex_ST;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                o.scrPos = ComputeScreenPos(o.pos);
                o.clr = v.color;
                return o;
            }

            half Screen(half hA, half hB){return 1.0-(1.0-hA)*(1.0-hB);}
            half Overlay(half hA, half hB){if(hA > 0.5)return 1.0-(1.0-hA)*(1.0-hB); else return hA*hB;}

            half4 frag (v2f i) : SV_Target
            {
                float2 textCoord = float2(floor(i.scrPos.x*_screenSize.x)/_screenSize.x, floor(i.scrPos.y*_screenSize.y)/_screenSize.y);

                textCoord.x -= _spriteRect.x;
                textCoord.y -= _spriteRect.y;

                textCoord.x /= _spriteRect.z - _spriteRect.x;
                textCoord.y /= _spriteRect.w - _spriteRect.y;

                half dst = clamp(distance(i.uv, half2(0.5, 0.5))*2.0, 0, 1);

                half2 p = i.uv * _fcapThresh + half2(i.clr.x, i.clr.y) * (1.-_fcapThresh) + (half2(0.5, 0.5) - i.uv)*0.2*pow(dst, 4);
                uint img = round(i.clr.z*25.0);

                half4 texCol = tex2D(_MainTex, half2((1.0/5.0)*(img%5) + p.x/5.0, (1.0/5.0)*(img/5) + p.y/5.0));
                half4 grabCol = tex2D(_GrabTexture, half2(i.scrPos.x, i.scrPos.y));
                half4 effectCol = half4(0.44705883, 0.9019608, 0.76862746, 1.0); // pebbles overseer color

                texCol = lerp(texCol,half4(Overlay(texCol.x, effectCol.x), Overlay(texCol.y, effectCol.y), Overlay(texCol.z, effectCol.z), texCol.w), lerp(1.0, 0.56, i.clr.w));
                texCol = half4(Screen(texCol.x, grabCol.x), Screen(texCol.y, grabCol.y), Screen(texCol.z, grabCol.z), texCol.w);

                texCol = lerp(texCol, effectCol, pow(1.0-i.clr.w, 1.5));

                half h = tex2D(_NoiseTex2, half2(textCoord.x*4, textCoord.y*8 - _RAIN*10)).x;
                if(fmod( round((textCoord.y - _RAIN*0.15) * 400), 3) == 0)
                    h += lerp(0.25, 0.05, i.clr.w);



                if(dst >= 1.0) return half4(0,0,0,0);

                h = pow(h, lerp(lerp(0.5, 10, i.clr.w), 0.5, dst));

                if(h > _hologramThreshold * i.clr.w)
                    return half4(0,0,0,0);
                else
                    return texCol;

            }
            ENDCG
        }
    }
}
