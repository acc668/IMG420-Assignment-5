shader_type canvas_item;

uniform float wave_intensity = 0.1;
uniform float wave_frequency = 10.0;
uniform vec4 color_start : source_color = vec4(1.0, 0.5, 0.0, 1.0);
uniform vec4 color_end : source_color = vec4(1.0, 0.0, 0.5, 1.0);

void fragment()
{
    vec2 uv = UV;

    uv.x += sin(uv.y * wave_frequency + TIME * 2.0) * wave_intensity;
    uv.y += cos(uv.x * wave_frequency * 0.5 + TIME * 1.5) * wave_intensity * 0.5;

    float gradient = uv.y;
    vec4 color = mix(color_start, color_end, gradient);

    float pulse = sin(TIME * 3.0) * 0.2 + 0.8;
    color.rgb *= pulse;

    vec4 tex_color = texture(TEXTURE, uv);
    COLOR = tex_color * color;
}
