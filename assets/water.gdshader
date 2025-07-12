shader_type canvas_item;

uniform vec4 WATER_TINT = vec4(0.4, 0.6, 0.9, 0.75);
uniform vec4 FOAM_TINT = vec4(1.0, 1.0, 1.0, 0.75);
uniform float FOAM_THICK = 0.05;

uniform float AMPLITUDE = 0.015;
uniform float FREQUENCY = 20;
uniform float SPEED = 2;

void fragment() {
	float wave = (sin(UV.x * FREQUENCY + TIME * SPEED) + 1.0) / 2.0 * AMPLITUDE;
	
	if (UV.y < wave) {
		COLOR = vec4(0.0, 0.0, 0.0, 0.0); // hide pixels
	} else {
		vec4 water_colour = WATER_TINT;
		if (UV.y < wave + FOAM_THICK) {
			water_colour = FOAM_TINT;
		}
		
		COLOR = water_colour;
	}
}
