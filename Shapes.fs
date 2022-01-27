#version 330 core
out vec4 FragColor;

in vec3 ourColor;
in vec2 TexCoord;

uniform float frame;

uniform int logicstate;
uniform int localstate;
uniform float localstatealpha;

float sdCircle(vec2 p,float r)
{
    return length(p)-r;
}

float sdBox(vec2 p,vec2 b)
{
    vec2 d=abs(p)-b;
    return length(max(d,0.))+min(max(d.x,d.y),0.);
}

float sdEquilateralTriangle(in vec2 p)
{
    const float k=sqrt(3.);
    p.x=abs(p.x)-1.;
    p.y=p.y+1./k;
    if(p.x+k*p.y>0.)p=vec2(p.x-k*p.y,-k*p.x-p.y)/2.;
    p.x-=clamp(p.x,-2.,0.);
    return-length(p)*sign(p.y);
}

float dot2(in vec2 v){return dot(v,v);}

float sdStairs(in vec2 p,in vec2 wh,in float n)
{
    vec2 ba=wh*n;
    float d=min(dot2(p-vec2(clamp(p.x,0.,ba.x),0.)),
    dot2(p-vec2(ba.x,clamp(p.y,0.,ba.y))));
    float s=sign(max(-p.y,p.x-ba.x));
    
    float dia=length(wh);
    p=mat2(wh.x,-wh.y,wh.y,wh.x)*p/dia;
    float id=clamp(round(p.x/dia),0.,n-1.);
    p.x=p.x-id*dia;
    p=mat2(wh.x,wh.y,-wh.y,wh.x)*p/dia;
    
    float hh=wh.y/2.;
    p.y-=hh;
    if(p.y>hh*sign(p.x))s=1.;
    p=(id<.5||p.x>0.)?p:-p;
    d=min(d,dot2(p-vec2(0.,clamp(p.y,-hh,hh))));
    d=min(d,dot2(p-vec2(clamp(p.x,0.,wh.x),hh)));
    
    return sqrt(d)*s;
}

float sdBezier(in vec2 pos,in vec2 A,in vec2 B,in vec2 C)
{
    vec2 a=B-A;
    vec2 b=A-2.*B+C;
    vec2 c=a*2.;
    vec2 d=A-pos;
    float kk=1./dot(b,b);
    float kx=kk*dot(a,b);
    float ky=kk*(2.*dot(a,a)+dot(d,b))/3.;
    float kz=kk*dot(d,a);
    float res=0.;
    float p=ky-kx*kx;
    float p3=p*p*p;
    float q=kx*(2.*kx*kx-3.*ky)+kz;
    float h=q*q+4.*p3;
    if(h>=0.)
    {
        h=sqrt(h);
        vec2 x=(vec2(h,-h)-q)/2.;
        vec2 uv=sign(x)*pow(abs(x),vec2(1./3.));
        float t=clamp(uv.x+uv.y-kx,0.,1.);
        res=dot2(d+(c+b*t)*t);
    }
    else
    {
        float z=sqrt(-p);
        float v=acos(q/(p*z*2.))/3.;
        float m=cos(v);
        float n=sin(v)*1.732050808;
        vec3 t=clamp(vec3(m+m,-n-m,n-m)*z-kx,0.,1.);
        res=min(dot2(d+(c+b*t.x)*t.x),
        dot2(d+(c+b*t.y)*t.y));
        // the third root cannot be the closest
        // res = min(res,dot2(d+(c+b*t.z)*t.z));
    }
    return sqrt(res);
}

// vec2 rotateUV(vec2 uv, float rotation)
// {
    //     float mid = 0.0;
    //     return vec2(
        //         cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
        //         cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
    //     );
// }

void main()
{
    
    vec2 iResolution=vec2(800,600);
    vec2 fragCoord=TexCoord;
    
    // -1 -> 1 local space, adjusted for aspect ratio
    vec2 uv=TexCoord*2-1.;
    
    ///rotation
    float rot=radians(1*360.)*abs(sin(frame*.05))*-1;
    mat2 m=mat2(cos(rot),-sin(rot),sin(rot),cos(rot));
    uv=m*uv;
    ///
    
    // local space x
    float aspect=iResolution.x/iResolution.y;
    uv.x*=aspect;
    
    float distance1;
    
    float thickness1=mix(.015,.1,abs(cos(frame*1)/1.));
    
    float fade1=.009;
    float fade2=mix(.03,1.035,sin(frame*.1));
    
    float circle=sdCircle(vec2(uv.x,uv.y),.75)*-1;
    vec2 box1vec=vec2(.75,.75);
    float box1=sdBox(uv,box1vec)*-1;
    float triangle=sdEquilateralTriangle(vec2(uv.x*1.5,uv.y*1.5+.3))*-1;// *1.5 -> scale from 1 to 0.75 ; +0.3 -> y translate
    vec2 box2vec=vec2(.75,.35);
    float box2=(sdBox(uv,box2vec)-.35)*-1;//(-0.5 from radius)
    vec2 whst=vec2(.25,.25);
    float stairs=(sdStairs(vec2(uv.x+.85,uv.y+.85),whst,7.)-.05)*-1;
    
    vec2 a1=vec2(cos(frame*1)/10.,(sin(frame*1)/5.)+.25);//sin(frame*.01)/5.0 lerp(a,b,sin(frame*.01));
    // vec2 a = vec2(0.05, 0.15);
    vec2 b1=vec2(.3,.95);
    // vec2 c = vec2(0.95,0.75);
    vec2 c1=vec2(mix(.75,1.35,cos(frame*3)/3.),mix(.75,1.35,sin(frame*3)/3.));
    // distance1 = sdBezier( uv, a*2-1, b*2-1, c*2-1 );
    float bezier=sdBezier(uv,a1*2-1,b1*2-1,c1*2-1);
    
    if(logicstate==0){
        if(localstate==0){
            distance1=circle;//(-0.5 from radius)
            
            thickness1=mix(.5,.015,localstatealpha);
        }
        if(localstate==1){
            distance1=mix(circle,box1,localstatealpha);
            thickness1=mix(.015,.5,localstatealpha);
            
        }
    }
    
    if(logicstate==1){
        if(localstate==0){
            distance1=box1;//(-0.5 from radius)
            
            thickness1=mix(.5,.015,localstatealpha);
        }
        if(localstate==1){
            distance1=mix(box1,triangle,localstatealpha);
            thickness1=mix(.015,.5,localstatealpha);
            
        }
    }
    
    if(logicstate==2){
        if(localstate==0){
            distance1=triangle;
            
            thickness1=mix(.5,.015,localstatealpha);
        }
        if(localstate==1){
            distance1=mix(triangle,box2,localstatealpha);
            thickness1=mix(.015,.5,localstatealpha);
            
        }
    }
    
    if(logicstate==3){
        if(localstate==0){
            distance1=box2;
            
            thickness1=mix(.5,.015,localstatealpha);
        }
        if(localstate==1){
            distance1=mix(box2,stairs,localstatealpha);
            thickness1=mix(.015,.5,localstatealpha);
            
        }
    }
    
    if(logicstate==4){
        if(localstate==0){
            distance1=stairs;
            
            thickness1=mix(.5,.015,localstatealpha);
        }
        if(localstate==1){
            distance1=mix(stairs,bezier,localstatealpha);
            thickness1=mix(.015,.001,localstatealpha);
            
        }
    }
    
    if(logicstate==5){
        
        thickness1=mix(.001,.015,localstatealpha);
        distance1=bezier;
        
    }
    
    if(logicstate>5&&logicstate<=7){
        
        thickness1=mix(.015,.017,localstatealpha*2-1);
        distance1=bezier;
        
    }
    
    vec3 color1=vec3(smoothstep(0.,fade1,distance1));
    // color1 *= vec3(smoothstep(thickness1 + fade1, thickness1, distance1));
    vec3 color2=color1*vec3(smoothstep(thickness1+fade1,thickness1,distance1));
    vec3 color3=color1*color2;
    
    vec3 color1f=vec3(smoothstep(0.,fade2,distance1));
    // color1 *= vec3(smoothstep(thickness1 + fade1, thickness1, distance1));
    vec3 color2f=color1*vec3(smoothstep(thickness1+fade2,thickness1,distance1));
    vec3 color3f=color1f*color2f;
    
    //     fragColor.rgb *= circleColor;
    vec3 color=vec3(1.,1.,1.);
    color3*=color;
    // color = vec3(0.9804, 0.4863, 0.0784);
    color=vec3(.9608,.9412,.9294);
    color3f*=color;
    
    FragColor=vec4(color3+color3f*.15,1.);//(distance2, 1.0) (color1, 1.0) (vec3(distance1), 1.0)
    
}

