/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 12.91839 ri + -0.01406 ci + -1.94956 si + 0.0 ti + 0.1229 fTempIi + 0.01462 ami + -157.86997;
asi = 0.0 hi + 0.0 li + -0.0 ri + -0.0 ci + 0.0 si + 0.0 ti + -0.0 fTempIi + 0.00014 ami + 0.0;
aTempOi = 0.0 hi + 0.0 li + 0.29119 ri + -0.09485 ci + -0.05068 si + -0.0 ti + 0.78193 fTempIi + -0.00042 ami + 6.9488;
fpdi = 0.0 hi + -0.0 li + 10.30089 ri + -24.06803 ci + -0.02241 si + -0.0 ti + -0.56341 fTempIi + 0.00017 ami + 289.57926;
fsi = 0.0 hi + -0.0 li + -7e-05 ri + -0.09248 ci + 1e-05 si + 0.0 ti + 0.00031 fTempIi + -0.0 ami + 2.09687;
fTempOi = 0.0 hi + 0.0 li + -0.28649 ri + 0.09419 ci + 0.04969 si + 0.0 ti + 0.24127 fTempIi + -0.00036 ami + 9.62386;
pi = 0.0 hi + 0.0 li + 194.26944 ri + 2.74095 ci + -19.15286 si + 0.0 ti + 0.0 fTempIi + 0.0 ami + 592.45841;

apdo = 0.0 ho + -0.0 lo + 10.98264 ro + 0.01663 co + -1.52334 so + 0.0 to + 0.11119 fTempIo + 0.0131 amo + -126.66838;
aso = 0.0 ho + 0.0 lo + 0.0 ro + -0.0 co + -0.0 so + 0.0 to + -0.0 fTempIo + 0.00014 amo + -0.0;
aTempOo = 0.0 ho + 0.0 lo + -0.33311 ro + 0.10185 co + 0.04386 so + 0.0 to + 0.72672 fTempIo + 0.00042 amo + 0.35644;
fpdo = 0.0 ho + -0.0 lo + 10.05321 ro + -23.98426 co + 0.01914 so + 0.0 to + -0.58299 fTempIo + -0.00019 amo + 288.32538;
fso = 0.0 ho + -0.0 lo + 0.0001 ro + -0.09244 co + -1e-05 so + 0.0 to + 0.00035 fTempIo + 0.0 amo + 2.09567;
fTempOo = 0.0 ho + 0.0 lo + 0.3269 ro + -0.09872 co + -0.04308 so + 0.0 to + 0.28743 fTempIo + 0.00039 amo + 11.98494;
po = 0.0 ho + -0.0 lo + 221.9625 ro + 2.74 co + -20.236 so + -0.0 to + -0.0 fTempIo + 0.0 amo + 894.11833;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s25i + s30i + s40i;
si =25s25i + 30s30i + 40s40i;
ti = 1;
apdi > 100;
apdi < 300;
asi > 0;
asi < 3;
aTempOi > 5;
aTempOi < 25;
fpdi > 100;
fpdi < 300;
fsi > 0;
fsi < 1.25;
fTempOi > 5;
fTempOi < 25;
pi > 0;
ri > 6;
ri < 14;
ci > 9;
ci < 60;
si > 25;
si < 60;
ti > 1;
ti < 3;
fTempIi > 5;
fTempIi < 25;

/* aTempOo < 11.4; */
1 =r6o + r8o + r10o + r12o + r14o;
ro =6r6o + 8r8o + 10r10o + 12r12o + 14r14o;
1 =s30o + s40o + s50o + s60o;
so =30s30o + 40s40o + 50s50o + 60s60o;
to = 3;
apdo > 100;
apdo < 300;
aso > 0;
aso < 3;
aTempOo > 5;
aTempOo < 25;
fpdo > 100;
fpdo < 300;
fso > 0;
fso < 1.25;
fTempOo > 5;
fTempOo < 25;
po > 0;
ro > 6;
ro < 14;
co > 9;
co < 60;
so > 25;
so < 60;
to > 1;
to < 3;
fTempIo > 5;
fTempIo < 25;
int hi,li,si,ti,ci,ri,ami;
int ho,lo,so,to,co,ro,amo;
bin s25i,s30i,s40i,s50i,s60i,r6i,r8i,r10i,r12i,r14i;
bin s25o,s30o,s40o,s50o,s60o,r6o,r8o,r10o,r12o,r14o;
