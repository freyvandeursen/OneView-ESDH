type LogoProps = {
  size: number;
};

export const Logo = ({ size }: LogoProps) => {
  return (
    <div>
      <img
        src={'/svglogo.svg'}
        alt="Logo"
        style={{ width: size, height: size }}
      />
    </div>
  );
};
