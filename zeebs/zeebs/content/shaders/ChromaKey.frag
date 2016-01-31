uniform sampler2D texture;
uniform vec4 color;

void main()
{
    vec4 pixel = texture2D(texture, gl_TexCoord[0].xy);
    if (pixel == vec4(1.0, 0.0, 1.0, 1.0))
    {
        gl_FragColor = color;
    }
    else
    {
        gl_FragColor = pixel;
    }
}