const getTimeSpan = (startDate: Date, endDate: Date): string => {
    const diff = endDate.getTime() - startDate.getTime();

    let remainingDiff = diff;

    const years = Math.floor(remainingDiff / (1000 * 60 * 60 * 24 * 365));
    remainingDiff -= years * (1000 * 60 * 60 * 24 * 365);

    const months = Math.floor(remainingDiff / (1000 * 60 * 60 * 24 * 30));
    remainingDiff -= months * (1000 * 60 * 60 * 24 * 30);

    const days = Math.floor(remainingDiff / (1000 * 60 * 60 * 24));
    remainingDiff -= days * (1000 * 60 * 60 * 24);

    const hours = Math.floor(remainingDiff / (1000 * 60 * 60));
    remainingDiff -= hours * (1000 * 60 * 60);

    const minutes = Math.floor(remainingDiff / (1000 * 60));

    const parts = [];
    if (years > 0) parts.push(`${years} ${years === 1 ? 'yr' : 'yrs'}`);
    if (months > 0) parts.push(`${months} ${months === 1 ? 'mo' : 'mos'}`);
    if (days > 0) parts.push(`${days} ${days === 1 ? 'day' : 'days'}`);
    if (hours > 0) parts.push(`${hours} ${hours === 1 ? 'hr' : 'hrs'}`);
    if (minutes > 0) parts.push(`${minutes} ${minutes === 1 ? 'min' : 'mins'}`);

    return parts.join(', ');
};

export {
    getTimeSpan
};