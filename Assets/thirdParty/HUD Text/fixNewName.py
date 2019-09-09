#coding:utf8
import os
os.system('find . -name "*.cs" | xargs grep -n "newName" > res.txt')
lines = open('res.txt').readlines()
for l in lines:
    fileName = l.split(':')[0]
    print fileName

    fixLines = open(fileName).readlines()
    c = 0
    for nl in fixLines:
        if nl.find('.newName') != -1:
            fixLines[c] = nl.replace('.newName', '.name')
        c += 1
    
    ret = str.join('',fixLines)

    nf = open(fileName, 'w')
    nf.write(ret)
    nf.close()



    

