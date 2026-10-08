shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_linear_mipmap;
uniform float blur : hint_range(0.0, 5.0) = 2.0;
uniform vec4 tint : source_color = vec4(0.0, 0.0, 0.0, 0.35);

void fragment() {
	vec3 col = textureLod(screen_texture, SCREEN_UV, blur).rgb;
	COLOR = vec4(mix(col, tint.rgb, tint.a), 1.0);
}