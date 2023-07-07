const fs = require('fs');

const Legacy = require('./LegacyOperational.json')
const Current = require('./Operationals.json')
const okList = [

]

Current.forEach(current => {
    const legacy = Legacy.find(l => l.type === current.type)
    if (!legacy || okList.includes(current.type)) {
        return;
    }
    if (current.operational[0] !== legacy.operational[0] || current.operational[1] !== legacy.operational[1]) {
        console.log(`${current.type} - OPERATIONAL MISMATCH L:${legacy.operational} != C:${current.operational}`)
    }
    const currentOperatorKeys = Object.keys(current.operators)
    currentOperatorKeys.forEach(key => {
        const legacyOperator = legacy.operators[key]
        if (!legacyOperator) {
            console.log(`${current.type} -  CURRENT EXTRA NATION ${key} ${current.operators[key]}`)
            return;
        }
        if (current.operators[key][0] !== legacyOperator[0] || current.operators[key][1] !== legacyOperator[1]) {
            console.log(`${current.type} - OPERATOR MISMATCH ${key} L:${legacyOperator} != C:${current.operators[key]}`)
        }
    })

    Object.keys(legacy.operators).filter(function(val) {
        return val != "CombinedJointTaskForcesBlue" && val != "CombinedJointTaskForcesRed" && currentOperatorKeys.indexOf(val) == -1;
       }).forEach(key => console.log(`${current.type} -  LEGACY EXTRA NATION ${key} ${legacy.operators[key]}`));
});