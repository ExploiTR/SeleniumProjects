import random

import matplotlib.pyplot as mat

dl_val = []
ul_val = []
t_val = []


def add_to_graph(_input_):
    _fra = _input_.split(",")
    dl_val.append(int(_fra[0]))
    ul_val.append(int(_fra[1]))
    tv = _fra[2].strip().split(" ")[1].split(":")
    t_val.append(tv[0] + ":" + tv[1])


def refineData():
    for i in range(len(dl_val)):
        if dl_val[i] > 85:
            dl_val[i] = random.randint(80, 85)
    for i in range(len(ul_val)):
        if ul_val[i] > 85:
            ul_val[i] = random.randint(80, 85)


inputFile = open("C:\\Users\\ExploiTR\\Desktop\\Record.txt", "r")

for input_ in inputFile:
    add_to_graph(input_)
    refineData()

fig, ax = mat.subplots()
mat.figlegend()
ax.set_xlabel("Time")
ax.set_ylabel("Speed")
ax.plot(t_val, dl_val, color='red', marker="o", label='s')
ax.plot(t_val, ul_val, color='blue', marker="o", label='sx')
mat.savefig('speedGraph.svg', format='svg', dpi=2000)
