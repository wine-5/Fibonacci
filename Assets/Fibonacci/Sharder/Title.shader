Shader "Custom/Title"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _GradientTop ("Gradient Top", Color) = (1,0.8,0.2,1)
        _GradientBottom ("Gradient Bottom", Color) = (1,0.4,0.1,1)
        _GlowIntensity ("Glow Intensity", Range(0,3)) = 1.5
        _GlowSpeed ("Glow Speed", Range(0,5)) = 2.0
        _OutlineColor ("Outline Color", Color) = (0.2,0.1,0.8,1)
        _WaveAmplitude ("Wave Amplitude", Range(0,0.1)) = 0.02
        _WaveFrequency ("Wave Frequency", Range(0,20)) = 10.0
        _WaveSpeed ("Wave Speed", Range(0,10)) = 3.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPos : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _GradientTop;
            fixed4 _GradientBottom;
            float _GlowIntensity;
            float _GlowSpeed;
            fixed4 _OutlineColor;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            
            v2f vert (appdata v)
            {
                v2f o;
                
                float wave = sin(v.uv.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
                v.vertex.y += wave;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                float gradientFactor = 1.0 - i.uv.y;
                fixed4 gradientColor = lerp(_GradientBottom, _GradientTop, gradientFactor);
                
                float glow = sin(_Time.y * _GlowSpeed) * 0.5 + 0.5;
                glow = pow(glow, 2) * _GlowIntensity;
                
                float sparkle = sin(i.uv.x * 15.0 + _Time.y * 3.0) * 0.3 + 0.7;
                
                col *= gradientColor;
                col.rgb += glow * 0.3 * _GradientTop.rgb;
                col.rgb += sparkle * 0.2 * _OutlineColor.rgb;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
