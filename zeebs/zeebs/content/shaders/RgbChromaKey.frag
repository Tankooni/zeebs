uniform sampler2D texture;
uniform vec4 color;

void main()
{
    vec4 pixel = texture2D(texture, gl_TexCoord[0].xy);
	float len = length(vec3(1.0, 0.0, 1.0) - pixel.xyz);

    if (len < 0.7)
    {
        gl_FragColor = vec4(color.xyz, pixel.w);
    }
    else
    {
        gl_FragColor = pixel;
    }
}