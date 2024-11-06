import { TasteSuggest } from '@/features/tasteSuggest/ui/TasteSuggest';
import { useIsMobile } from '@/shared/hooks/useIsMobile';

type RightSideLayoutProps = {
  className?: string;
  children?: React.ReactNode;
} & React.HTMLAttributes<HTMLElement>;

const RightSideLayout = ({
  children,
  className,
  ...props
}: RightSideLayoutProps) => {
  const isMobile = useIsMobile();

  if (isMobile) {
    return null;
  }

  return (
    <aside
      className={`hidden lg:block fixed top-16 right-0 w-64 xl:w-96 h-[calc(100vh-4rem)] overflow-y-auto ${className}`}
      {...props}
    >
      <TasteSuggest isOpen={true} onClose={() => {}} />
      {children}
    </aside>
  );
};

export default RightSideLayout;
